using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui.Appearance;
using XIVUniPF.Classes;
using XIVUniPF.Views;
using XIVUniPF_Core;

namespace XIVUniPF
{
    public partial class App : Application
    {
        public static readonly string Version = "v0.2.1";


        public new static App Current => (App)Application.Current;

        public static AppConfig Config => Current._config!;

        private static Mutex? _processMutex;


        public IServiceProvider Services => services!;


        private YamlConfigManager<AppConfig>? configManager;

        private AppConfig? _config;

        private TaskbarIcon? trayIcon;

        private Timer? refreshTimer;

        private PipeIPC? ipc;

        private IServiceProvider? services;


        public static void ShowMainWindow()
        {
            Current.Dispatcher.Invoke(() =>
            {
                var window = Current.MainWindow;
                // 在 Debug 模式中 window 可能不是 MainWindow 而是 VS 的辅助窗口
                // 所以作出特判
                if (window == null || window is not Views.MainWindow)
                {
                    window = Current.MainWindow = new MainWindow();
                    window.Show();
                    return;
                }
                if (window.WindowState == WindowState.Minimized)
                    window.WindowState = WindowState.Normal;
                window.Activate();
            });
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            await EnsureAppSingleton();

            services = new ServiceCollection()
                .AddSingleton<IPFService, PFService>()
                .BuildServiceProvider();

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
                    _ = Services.GetRequiredService<IPFService>().Refresh();
            }, null, TimeSpan.FromSeconds(Config.AutoRefreshInterval), TimeSpan.FromSeconds(Config.AutoRefreshInterval));
            Config.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AppConfig.AutoRefreshInterval))
                    refreshTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(Config.AutoRefreshInterval));
            };

            Current.StartupUri = new Uri("Views/MainWindow.xaml", UriKind.Relative);
        }

        // 确保App单例运行
        // 如果后台已有实例运行则唤醒它
        private async Task EnsureAppSingleton()
        {
            ipc = new PipeIPC("XIVUniPF_IPC");
            _processMutex = new Mutex(true, "XIVUniPFApp", out bool success);
            // 加锁失败，说明已有实例运行
            if (!success)
            {
                await ipc.SendMessageAsync("show");
                Shutdown();
                Environment.Exit(0);
            }
            ipc.StartServer();
            ipc.MessageReceived += (msg) =>
            {
                if (msg == "show")
                    ShowMainWindow();
            };
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            configManager!.Save(_config!);
        }
    }
}
