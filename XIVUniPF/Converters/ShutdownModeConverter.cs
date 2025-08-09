using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace XIVUniPF.Converters
{
    internal class ShutdownModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool)
                throw new InvalidCastException("输入必须是 bool 类型");

            return (bool)value ? ShutdownMode.OnExplicitShutdown : ShutdownMode.OnLastWindowClose;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}