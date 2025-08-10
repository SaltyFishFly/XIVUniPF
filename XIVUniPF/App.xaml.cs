using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Toolkit.Uwp.Notifications;
using System.IO;
using System.Net.Http;
using System.Security.Policy;
using System.Text.Json;
using System.Windows;
using XIVUniPF.Classes;

namespace XIVUniPF
{
    public partial class App : Application
    {
        public static readonly string Version = "v0.1.0";

        public static AppConfig Config => ((App)Current)._config!;

        private YamlConfigManager<AppConfig>? configManager;
        private AppConfig? _config;
        private TaskbarIcon? trayIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // 同步系统主题
            // 实验性 API
#pragma warning disable WPF0001
            ThemeMode = ThemeMode.System;
#pragma warning restore WPF0001

            string cfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.yaml");
            configManager = new YamlConfigManager<AppConfig>(cfgPath);
            _config = configManager.Load();

            trayIcon = (TaskbarIcon)FindResource("Taskbar");

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
                using var http = new HttpClient();
                http.DefaultRequestHeaders.UserAgent.ParseAdd("XIVUniPF-Updater");
                var json = await http.GetStringAsync("https://api.github.com/repos/SaltyFishFly/XIVUniPF/releases/latest");
                using var doc = JsonDocument.Parse(json);
                var latestTag = doc.RootElement.GetProperty("tag_name").GetString();

                if (string.IsNullOrWhiteSpace(latestTag))
                    return;

                // 去掉 'v'
                var latestVersion = latestTag.TrimStart('v');
                var currentVersion = App.Version.TrimStart('v');

                if (IsNewerVersion(latestVersion, currentVersion))
                {
                    // 弹出通知
                    new ToastContentBuilder()
                        .AddText("发现新版本 🎉")
                        .AddText($"当前版本: {App.Version}，最新版本: {latestTag}")
                        .SetProtocolActivation(new Uri("https://github.com/SaltyFishFly/XIVUniPF/releases"))
                        .Show();
                }
            }
            catch { }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            configManager!.Save(_config!);
        }
    }
}
