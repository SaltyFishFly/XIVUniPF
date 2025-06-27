using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace XIVUniPF.Converters
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool)
                throw new InvalidCastException("加载情况必须是 bool 类型");

            return (bool) value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
