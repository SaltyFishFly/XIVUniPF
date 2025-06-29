using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace XIVUniPF.Converters
{
    internal class CategoryToIconConverter : IValueConverter
    {
        private static readonly Dictionary<string, BitmapImage> Map = new()
        {
            { "deepdungeon", load("Assets/DutyIcons/DeepDungeon.png") },
            { "dungeons", load("Assets/DutyIcons/dungeons.png") },
            { "dutyroulette", load("Assets/DutyIcons/dutyroulette.png") },
            { "adventuringforays", load("Assets/DutyIcons/eureka.png") },
            { "gatheringforays", load("Assets/DutyIcons/exploratorymissions.png") },
            { "fates", load("Assets/DutyIcons/fates.png") },
            { "goldsaucer", load("Assets/DutyIcons/goldsaucer.png") },
            { "guildhests", load("Assets/DutyIcons/guildhests.png") },
            { "highendduty", load("Assets/DutyIcons/highendduty.png") },
            { "none", load("Assets/DutyIcons/none.png") },
            { "pvp", load("Assets/DutyIcons/pvp.png") },
            { "raids", load("Assets/DutyIcons/raids.png") },
            { "seasonal", load("Assets/DutyIcons/seasonal.png") },
            { "thehunt", load("Assets/DutyIcons/thehunt.png") },
            { "treasurehunt", load("Assets/DutyIcons/treasurehunt.png") },
            { "trials", load("Assets/DutyIcons/trials.png") },
            { "weeklybingo", load("Assets/DutyIcons/weeklybingo.png") },
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
            var img = Map.TryGetValue(key, out var value) ? value : Map["none"];
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static BitmapImage load(string path)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.EndInit();
            return img;
        }
    }
}
