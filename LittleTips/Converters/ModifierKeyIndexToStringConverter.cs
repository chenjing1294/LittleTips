using System;
using System.Globalization;
using System.Windows.Data;

namespace LittleTips.Converters
{
    public class ModifierKeyIndexToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                switch (index)
                {
                    case 0: //Ctrl
                        return "Ctrl";
                        break;
                    case 1: //Alt
                        return "Alt";
                        break;
                    case 2: //Shift
                        return "Shift";
                        break;
                    case 3: //Win
                        return "Win";
                        break;
                    default:
                        return "Ctrl";
                        break;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}