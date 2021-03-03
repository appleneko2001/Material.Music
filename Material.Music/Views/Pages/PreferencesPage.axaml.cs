using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Material.Music.Core;
using Material.Music.ViewModels;

namespace Material.Music.Views.Pages
{
    public class PreferencesPage : UserControl
    {
        public PreferencesPage()
        {
            this.InitializeComponent();

            DataContext = new PreferencesViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void TextLink2_OnPointerPressed(object? sender, PointerPressedEventArgs e) =>
            Utils.OpenBrowserForVisitSite("https://www.un4seen.com/bass.html");

        private void TextLink3_OnPointerPressed(object? sender, PointerPressedEventArgs e) =>
            Utils.OpenBrowserForVisitSite("https://github.com/AvaloniaUtils/material.avalonia");

        private void TextLink1_OnPointerPressed(object? sender, PointerPressedEventArgs e) =>
            Utils.OpenBrowserForVisitSite("https://github.com/appleneko2001");

        private void GithubLink1_OnClick(object? sender, RoutedEventArgs e) =>
            Utils.OpenBrowserForVisitSite("https://github.com/appleneko2001");

        private void TwitterButton_OnClick(object? sender, RoutedEventArgs e) =>
            Utils.OpenBrowserForVisitSite("https://twitter.com/pigeonmaster01");

        private void DiscordButton_OnClick(object? sender, RoutedEventArgs e) =>
            Utils.OpenBrowserForVisitSite("https://discord.com/invite/5xKSXkm");
    }
}
