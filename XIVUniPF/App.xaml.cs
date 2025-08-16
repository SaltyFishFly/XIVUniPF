using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using Wpf.Ui.Appearance;
using XIVUniPF.Classes;
using XIVUniPF_Core;

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
        private Timer? refreshTimer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 确保单例运行
            _processMutex = new Mutex(true, "XIVUniPFApp", out bool success);
            if (!success)
            {
                // 唤醒后台运行的实例
                Shutdown();
                Environment.Exit(0);
            }

            // 加载设置
            string cfgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.yaml");
            configManager = new YamlConfigManager<AppConfig>(cfgPath);
            _config = configManager.Load();

            // 设置主题
            ApplicationThemeManager.Apply(Config.ThemeIndex switch
            {
                0 => ApplicationTheme.Light,
                1 => ApplicationTheme.Dark,
                _ => throw new IndexOutOfRangeException("ThemeIndex超出范围")
            });

            // 托盘图标
            trayIcon = (TaskbarIcon)FindResource("Taskbar");

            // 检查更新
            if (Config.AutoCheckUpdates)
                _ = Utils.CheckUpdate();

            // 自动刷新
            refreshTimer = new Timer(_ =>
            {
                bool notVisible = Dispatcher.Invoke(() =>
                {
                    var window = Current.MainWindow;
                    return
                        window == null ||
                        !window.IsVisible ||
                        window.WindowState == WindowState.Minimized ||
                        window.Opacity == 0;
                });
                if (Config.AutoRefresh && notVisible)
                    _ = PFService.Instance.Update();
            }, null, TimeSpan.FromSeconds(Config.AutoRefreshInterval), TimeSpan.FromSeconds(Config.AutoRefreshInterval));
            Config.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AppConfig.AutoRefreshInterval))
                    refreshTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(Config.AutoRefreshInterval));
            };
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            configManager!.Save(_config!);
        }
    }
}
