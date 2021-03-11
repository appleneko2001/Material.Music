using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Diagnostics;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Material.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using Material.Dialog;
using Material.Icons;
using Material.Music.Core;
using Material.Music.Core.Engine;
using Material.Music.Core.Enums;
using Material.Music.Core.EventArgs;
using Material.Music.Core.LocalMedia;
using Material.Music.ViewModels;
using Material.Music.Views.Controls;
using Material.Music.Views.Interfaces;

namespace Material.Music.Views
{
    public class MainWindow : Window
    {
        #region Page control members

        private Grid AppBarTop;
        private Grid PlayInstantlyDropHint;
        private StackPanel FloatingButtonContainer;
        private Carousel PageController;
        private ListBox LeftDrawerList;
        private ToggleButton DrawerToggle;
        private FloatingButton CreatePlaylistFAB;
        private ReversibleStackPanel ContextToolbarPanel;

        private static MainWindow _instance;
        
        #endregion

        private const double AppBarMinimized = 64;
        private const double AppBarWithFabs = 96;

        public static MainWindow Instance => _instance;

#pragma warning disable IDE0002
        public MainWindow()
        {
            void InitializeComponent() => AvaloniaXamlLoader.Load(this);

            InitializeComponent();

            AppBarTop = this.Get<Grid>(nameof(AppBarTop));

            PlayInstantlyDropHint = this.Get<Grid>(nameof(PlayInstantlyDropHint));
            var playerDockPanel = this.Get<PlayerDock>("PlayerDockPanel");

            FloatingButtonContainer = this.Get<StackPanel>(nameof(FloatingButtonContainer));

            PageController = this.Get<Carousel>(nameof(PageController));

            playerDockPanel.AddHandler(DragDrop.DragEnterEvent, (o, e) => SetDragDropTipVisibility(true));
            playerDockPanel.AddHandler(DragDrop.DragOverEvent, (o, e) => SetDragDropTipVisibility(true));
            playerDockPanel.AddHandler(DragDrop.DragLeaveEvent, (o, e) => SetDragDropTipVisibility(false));
            playerDockPanel.AddHandler(DragDrop.DropEvent, OnFileDroppedToPlayerDock);

            DrawerToggle = this.Get<ToggleButton>(nameof(DrawerToggle));

            ContextToolbarPanel = this.Get<ReversibleStackPanel>(nameof(ContextToolbarPanel));

            LeftDrawerList = this.Get<ListBox>(nameof(LeftDrawerList));
            LeftDrawerList.PointerReleased += OnLeftDrawerSelectionChanged;

            // Create instance FAB Buttons and bind handler
            CreatePlaylistFAB = FloatingButton.CreateFloatingButton(MaterialIconKind.PlaylistPlus, "CREATE PLAYLIST");
            CreatePlaylistFAB.Click += (sender, args) => CreatePlaylist();
            ChangeFloatingActionButtons();
            
            //Observe ClientSize
            ClientSizeProperty.Changed.AddClassHandler<MainWindow>((window, args) => window.ClientSizeChanged(args));
            
            DataContext = PlayerContext.GetInstance();

            BassEngine.GetInstance().OnChannelStatusChanged+= OnOnChannelStatusChanged;
#if DEBUG
            this.AttachDevTools();
#endif
            _instance = this;
        }

        private async void OnOnChannelStatusChanged(object? sender, ChannelStatusEventArgs e)
        {
            if (e.Status is ChannelStatusEnums.Loaded)
            {
                /*await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var notification = new DesktopNotifications.Notification()
                    {
                        Title = "Material.Music", 
                        Body =  $"Now playing: {e.Playable.Title}",
                    };
                    App.NotificationManager.ShowNotification(notification);
                });*/
            }
        }

        // Show create playlist dialog. After submit information will create playlist. 
        private async void CreatePlaylist()
        {
            var dialog = DialogHelper.CreateTextFieldDialog(new TextFieldDialogBuilderParams()
            {
                Width = 460, 
                Borderless = true,
                StartupLocation = WindowStartupLocation.CenterOwner,
                ContentHeader = "Create Playlist",
                PositiveButton = new DialogResultButton()
                {
                    Content = "CREATE",
                    Result = "submit"
                },
                NegativeButton = new DialogResultButton()
                {
                    Content = "CANCEL",
                    Result = "cancel",
                },
                TextFields = new []
                {
                    new TextFieldBuilderParams()
                    {
                        Label = "Playlist name",
                        DefaultText = "Playlist",
                    }
                }
            });
            var result = await dialog.ShowDialog(this);
            if (result.GetResult == "submit")
            {
                var context = PlayerContext.GetInstance();
                context.Playlists.Add(new LocalPlaylist()
                {
                    Name = result.GetFieldsResult()[0].Text
                });
            }
        }

#pragma warning restore IDE0002


        // Update FABs extend status when window resized
        private void ClientSizeChanged(AvaloniaPropertyChangedEventArgs obj)
        {
            var size = obj.NewValue as Size?;
            if (size != null)
            {
                UpdateFloatingButtonExtendedStatus(size.Value);
            }
        }

        // Update FABs extend status
        private void UpdateFloatingButtonExtendedStatus(Size size)
        {
            var widerWindow = size.Width > 800;
            foreach (var control in FloatingButtonContainer.Children)
            {
                var fab = control as FloatingButton;
                fab?.SetValue(FloatingButton.IsExtendedProperty, widerWindow);
            }
        }
        
        // Attach and detach floating button when pages changed
        public void ChangeFloatingActionButtons()
        {
            FloatingButtonContainer.Children.Clear(); // Remove all FABs

            // When current page is homepage
            if(PageController.SelectedIndex == 0)
                FloatingButtonContainer.Children.Add(CreatePlaylistFAB);

            UpdateFloatingButtonExtendedStatus(ClientSize);
            UpdateAppBarStatus();
        }

        public void UpdateAppBarStatus()
        {
            AppBarTop.Height = FloatingButtonContainer.Children.Count > 0 ? AppBarWithFabs : AppBarMinimized;
        }
        
        public void ChangeActionMenus()
        {
            ContextToolbarPanel.Children.Clear();

            var currentPageActionMenus = PageController.SelectedItem as IHasActionBarMenus;

            if (currentPageActionMenus != null)
            {
                var actionMenus = currentPageActionMenus.GetActionBarMenus();
                ContextToolbarPanel.Children.AddRange(actionMenus);
            }
        }

        // Switch page when pressed left drawer item
        private void OnLeftDrawerSelectionChanged(object sender, PointerReleasedEventArgs e)
        {
            var list = sender as ListBox;
            
            ChangePage(list.SelectedIndex);
        }

        public void ChangePage(int index)
        {
            var oldSelectedPage = PageController.SelectedIndex;
            PageController.SelectedIndex = index;
            
            DrawerToggle.IsChecked = false;
            
            if(oldSelectedPage != index)
            {
                ChangeFloatingActionButtons();
                ChangeActionMenus();
            }
        }

        private void OnFileDroppedToPlayerDock (object sender, DragEventArgs e)
        {
            SetDragDropTipVisibility(false);
            if (e.DragEffects.HasFlag(DragDropEffects.Move) || e.DragEffects.HasFlag(DragDropEffects.Copy))
            {
                var dragDropData = e.Data.Get(DataFormats.FileNames) as List<string>; 
                var item = dragDropData.First();
                Task.Run(() =>
                {
                    try
                    {
                        PlayerCommands.PlayLocalFile(item);
                    }
                    catch
                    {

                    }
                });
            }
        }

        private void SetDragDropTipVisibility(bool visible) => PlayInstantlyDropHint.IsVisible = visible;
    }
}