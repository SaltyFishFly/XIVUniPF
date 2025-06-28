using System.Globalization;
using System.Windows.Data;

namespace XIVUniPF.Converters
{
    internal class UsernameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var username = values[0]?.ToString() ?? string.Empty;
            var homeworld = values[1]?.ToString() ?? string.Empty;
            return $"{username}@{homeworld}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
