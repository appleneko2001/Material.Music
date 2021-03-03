using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Material.Dialog;
using Material.Dialog.Enums;
using Material.Icons;
using Material.Icons.Avalonia;
using Material.Music.ViewModels;
using Material.Music.Views.Dialogs;
using Material.Music.Views.Interfaces;
using Material.Music.Models;

namespace Material.Music.Views.Pages
{
    public class SubtitlePage : UserControl, IHasActionBarMenus
    {
        private Control[] _actionBarMenus;
        
        public SubtitlePage()
        {
            this.InitializeComponent();

            _actionBarMenus = new[]
            {
                CreateActionMenu(MaterialIconKind.Magnify, SearchSubtitle, "Search subtitle / lyric")
            };
            
            DataContext = PlayerContext.GetInstance();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public Control[] GetActionBarMenus() => _actionBarMenus;

        public async void SearchSubtitle()
        {
            var dialog = DialogHelper.CreateCustomDialog(new CustomDialogBuilderParams()
            {
                Borderless = true,
                DialogButtons = DialogHelper.CreateSimpleDialogButtons(DialogButtonsEnum.OkCancel),
                ContentHeader = "Search subtitle of track",
                Width = 800,
                Content = new SearchSubtitleDialog()
            });
            await dialog.ShowDialog(MainWindow.Instance);
        }

        public static Control CreateActionMenu(MaterialIconKind iconKind, Action onClick, string tip)
        {
            var b = new Button()
            {
                Classes = Classes.Parse("Flat"),
                Padding = new Thickness(4),
                Margin = new Thickness(8, 0),
                Content = new MaterialIcon()
                {
                    Kind = iconKind
                },
            };
            b.Click += (a, b) => onClick();
            b.SetValue(ToolTip.TipProperty, tip);
            return b;
        }
    }
}
