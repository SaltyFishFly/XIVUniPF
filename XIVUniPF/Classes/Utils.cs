using Microsoft.Toolkit.Uwp.Notifications;
using System.Net.Http;
using System.Text.Json;

namespace XIVUniPF.Classes
{
    public static class Utils
    {
        // 获取的 id 是我瞎编的
        // 为什么猪区在猫区前面，因为作者是猪区人（?）
        public static int GetDatacenterId(string name)
        {
            return name switch
            {
                "陆行鸟" => 1,
                "莫古力" => 2,
                "猫小胖" => 3,
                "豆豆柴" => 4,
                _ => 5
            };
        }

        // 检查更新
        public static async Task CheckUpdate()
        {
            static bool IsNewerVersion(string latest, string current)
            {
                var latestParts = latest.Split('.');
                var currentParts = current.Split('.');
                for (int i = 0; i < Math.Max(latestParts.Length, currentParts.Length); i++)
                {
                    int latestNum = i < latestParts.Length && int.TryParse(latestParts[i], out var ln) ? ln : 0;
                    int currentNum = i < currentParts.Length && int.TryParse(currentParts[i], out var cn) ? cn : 0;
                    if (latestNum > currentNum) return true;
                    if (latestNum < currentNum) return false;
                }
                return false;
            }

            try
            {
                // 从 GitHub API 获取最新 release
                var handler = new HttpClientHandler
                {
                    UseProxy = App.Config.UseSystemProxyCheckUpdates
                };
                using var http = new HttpClient(handler);
                http.DefaultRequestHeaders.UserAgent.ParseAdd("XIVUniPF-Updater");
                var json = await http.GetStringAsync("https://api.github.com/repos/SaltyFishFly/XIVUniPF/releases/latest");
                using var doc = JsonDocument.Parse(json);

                var latestTag = doc.RootElement.GetProperty("tag_name").GetString();
                if (string.IsNullOrWhiteSpace(latestTag))
                    return;
                if (IsNewerVersion(latestTag.TrimStart('v'), App.Version.TrimStart('v')))
                {
                    // 弹出通知
                    new ToastContentBuilder()
                        .AddText("发现新版本 🎉")
                        .AddText($"当前版本: {App.Version}，最新版本: {latestTag}")
                        .SetProtocolActivation(new Uri("https://github.com/SaltyFishFly/XIVUniPF/releases"))
                        .Show();
                }
            }
            catch (Exception ex)
            {
                new ToastContentBuilder()
                    .AddText("检查更新失败")
                    .AddText("可能是与GitHub的连接不稳定，请稍后再试。")
                    .AddText($"错误信息: {ex.Message}")
                    .Show();
            }
        }
    }
}
