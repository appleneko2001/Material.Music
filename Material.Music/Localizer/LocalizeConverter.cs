using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Material.Music.Localizer
{
    public class LocalizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var keyToUse = value as string;
            
            var context = parameter as string;
            if (!string.IsNullOrWhiteSpace(context))
                keyToUse = $"{context}.{value}";

            return Localizer.Instance[keyToUse];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}