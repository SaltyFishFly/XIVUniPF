using System.Globalization;
using System.Windows.Data;

namespace XIVUniPF.Converters
{
    internal class UpdatedTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DateTime)
                throw new ArgumentException("时间必须是DateTime类型");

            return (DateTime.Now - (DateTime)value) switch
            {
                var ts when ts.TotalMinutes > 1 => $"{(int)ts.TotalMinutes} 分钟前更新",
                var ts when ts.TotalSeconds > 1 => $"{(int)ts.TotalSeconds} 秒前更新",
                _ => "刚刚更新"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
