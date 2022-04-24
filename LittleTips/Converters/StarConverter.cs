using System;
using System.Globalization;
using System.Windows.Data;

namespace LittleTips.Converters
{
    public class StarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isFavorite)
            {
                if (isFavorite)
                {
                    return "&#xe00a;";
                }
            }

            return "&#xe1ce;";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}