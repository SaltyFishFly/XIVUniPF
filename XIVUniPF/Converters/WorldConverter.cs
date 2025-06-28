using System.Globalization;
using System.Windows.Data;

namespace XIVUniPF.Converters
{
    internal class WorldConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var datacenter = value[0]?.ToString() ?? "未知";
            var world = value[1]?.ToString() ?? "未知";
            return $"{datacenter} - {world}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
