using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Toolkit.Uwp.Notifications;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using Wpf.Ui.Appearance;
using XIVUniPF.Classes;

namespace XIVUniPF
{
    public partial class App : Application
    {
        public static readonly string Version = "v0.2.0";

        private static Mutex? _processMutex;
        public static AppConfig Config => ((App)Current)._config!;

        private YamlConfigManager<AppConfig>? configManager;
        private AppConfig? _config;
        private TaskbarIcon? trayIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 确保单例运行
            _processMutex = new Mutex(true, "XIVUniPFApp", out bool success);
            if (!success)
            {
                Shutdown();
                return;
            }

            // 加载设置
            string cfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.yaml");
            configManager = new YamlConfigManager<AppConfig>(cfgPath);
            _config = configManager.Load();

            // 设置主题
            var theme = App.Config.ThemeIndex switch
            {
                0 => ApplicationTheme.Light,
                1 => ApplicationTheme.Dark,
                _ => throw new IndexOutOfRangeException("ThemeIndex超出范围")
            };
            ApplicationThemeManager.Apply(theme);

            // 托盘图标
            trayIcon = (TaskbarIcon)FindResource("Taskbar");

            // 检查更新
            if (Config.AutoCheckUpdates)
                _ = CheckUpdate();
        }

        private async Task CheckUpdate()
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
                    UseProxy = Config.UseSystemProxy
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

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            configManager!.Save(_config!);
        }
    }
}
