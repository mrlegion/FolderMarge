using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FolderMarge
{
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool str = (bool?)value ?? true;
            return str.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
