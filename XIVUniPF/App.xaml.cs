using Hardcodet.Wpf.TaskbarNotification;
using System.IO;
using System.Windows;
using XIVUniPF.Classes;

namespace XIVUniPF
{
    public partial class App : Application
    {
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
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            configManager!.Save(_config!);
        }
    }
}
