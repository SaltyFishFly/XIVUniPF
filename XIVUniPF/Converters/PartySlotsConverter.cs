using System.Globalization;
using System.Windows.Data;

namespace XIVUniPF.Converters
{
    internal class PartySlotsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var used = values[0] as int? ?? 0;
            var total = values[1] as int? ?? 0;
            return $"{used} / {total}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
