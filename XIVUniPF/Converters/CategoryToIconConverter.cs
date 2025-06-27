using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace XIVUniPF.Converters
{
    internal class CategoryToIconConverter : IValueConverter
    {
        private static readonly Dictionary<string, string> Map = new()
        {
            { "deepdungeon", "Assets/DutyIcons/DeepDungeon.png" },
            { "dungeons", "Assets/DutyIcons/dungeons.png" },
            { "dutyroulette", "Assets/DutyIcons/dutyroulette.png" },
            { "eureka", "Assets/DutyIcons/eureka.png" },
            { "exploratorymissions", "Assets/DutyIcons/exploratorymissions.png" },
            { "fates", "Assets/DutyIcons/fates.png" },
            { "goldsaucer", "Assets/DutyIcons/goldsaucer.png" },
            { "guildhests", "Assets/DutyIcons/guildhests.png" },
            { "highendduty", "Assets/DutyIcons/highendduty.png" },
            { "none", "Assets/DutyIcons/none.png" },
            { "pvp", "Assets/DutyIcons/pvp.png" },
            { "raids", "Assets/DutyIcons/raids.png" },
            { "seasonal", "Assets/DutyIcons/seasonal.png" },
            { "thehunt", "Assets/DutyIcons/thehunt.png" },
            { "treasurehunt", "Assets/DutyIcons/treasurehunt.png" },
            { "trials", "Assets/DutyIcons/trials.png" },
            { "weeklybingo", "Assets/DutyIcons/weeklybingo.png" },
        };

        /// <summary>
        /// 将 Category 转换为图标路径
        /// 如 "raids" 转换为 "/Assets/DutyIcons/raids.png"
        /// </summary>
        /// <param name="category">招募类型</param>
        /// <param name="targetType">没啥用</param>
        /// <param name="parameter">没啥用</param>
        /// <param name="culture">没啥用</param>
        /// <returns cref="BitmapImage">图标</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object Convert(object category, Type targetType, object parameter, CultureInfo culture)
        {
            if (category is not string)
                throw new InvalidCastException("招募类型必须是字符串");

            var key = ((string)category).ToLower();
            var path = Map.TryGetValue(key, out var value) ? value : "Assets/DutyIcons/none.png";
            // 加载图标
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.EndInit();
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
