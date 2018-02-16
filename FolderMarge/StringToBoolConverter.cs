using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FolderMarge
{
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string) value;
            return !((string.IsNullOrEmpty(str)) && (string.IsNullOrWhiteSpace(str)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
