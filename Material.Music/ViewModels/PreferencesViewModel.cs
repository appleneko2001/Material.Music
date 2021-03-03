using System.Collections.ObjectModel;
using Avalonia;
using Material.Icons;
using Material.Styles.Assists;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;

namespace Material.Music.ViewModels
{
    public class PreferencesViewModel : ViewModelBase
    {
        private static PaletteHelper m_paletteHelper;
        private static PaletteHelper PaletteHelper 
        { 
            get 
            { 
                if (m_paletteHelper is null) 
                    m_paletteHelper = new PaletteHelper();
                return m_paletteHelper;
            } 
        }
        
        public PreferencesViewModel()
        {
            PreferencesView = new ObservableCollection<object>(new[]
            {
                new ToggleSetterViewModel("Switch transitions", "Transitions and animations will be disabled for better performance",
                    null, "Transitions" , defaultValue:false,null, e => TransitionAssist.SetDisableTransitions(App.Instance.MainWindow, e)),
                new ToggleSetterViewModel("Dark theme", null, MaterialIconKind.ThemeLightDark, "DarkTheme",false, null, (e) => ApplyTheme(e))
            });
        }

        public ObservableCollection<object> PreferencesView { get; private set; }

        private static void ApplyTheme(bool enableDarkTheme)
        {
            var theme = PaletteHelper.GetTheme();
            theme.SetBaseTheme(enableDarkTheme ? 
                BaseThemeMode.Dark.GetBaseTheme() : 
                BaseThemeMode.Light.GetBaseTheme());
            PaletteHelper.SetTheme(theme);
        }
    }
}