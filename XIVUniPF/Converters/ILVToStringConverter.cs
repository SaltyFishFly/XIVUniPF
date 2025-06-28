using System.Globalization;
using System.Windows.Data;

namespace XIVUniPF.Converters
{
    internal class ILVToStringConverter : IValueConverter
    {
        public object Convert(object ilv, Type targetType, object parameter, CultureInfo culture)
        {
            if (ilv is not int)
                throw new InvalidCastException("ILV的类型必须为整数");

            return (int)ilv != 0 ? $"最低品级：{ilv}" : "最低品级：无";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
