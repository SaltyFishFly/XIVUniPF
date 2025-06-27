using System.Globalization;
using System.Windows.Data;

namespace XIVUniPF.Converters
{
    internal class TimeLeftToStringConverter : IValueConverter
    {
        public object Convert(object time, Type targetType, object parameter, CultureInfo culture)
        {
            if (time is not float)
                throw new InvalidCastException("时间类型必须为浮点数");

            var sec = (int)(float)time;
            return $"剩余{sec / 60 + 1}分";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
