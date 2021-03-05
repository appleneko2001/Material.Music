// https://www.sakya.it/wordpress/avalonia-ui-framework-localization/

using System;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;

namespace Material.Music.Localizer
{
    public class LocalizeExtension : MarkupExtension
    {
        public LocalizeExtension()
        {
            
        }
        
        public LocalizeExtension(string key)
        {
            this.Key = key;
        }

        public string Key { get; set; }

        public string Context { get; set; }
        
        public bool UseBinding { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var keyToUse = Key;
            if (!string.IsNullOrWhiteSpace(Context))
                keyToUse = $"{Context}.{Key}";

            if (UseBinding)
            {
                var binding = new ReflectionBindingExtension($"[{keyToUse}]")
                {
                    Mode = BindingMode.OneWay,
                    Source = Localizer.Instance,
                };

                var result = binding.ProvideValue(serviceProvider); 
                return result;
            }
            else
            {
                return Localizer.Instance[keyToUse];
            }
        }
    }
}
