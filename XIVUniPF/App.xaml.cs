using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;

namespace XIVUniPF
{
    public partial class App : Application
    {
        private TaskbarIcon? _trayIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _trayIcon = (TaskbarIcon)FindResource("Taskbar");

            // 同步系统主题
            // 实验性 API
#pragma warning disable WPF0001
            ThemeMode = ThemeMode.System;
#pragma warning restore WPF0001
        }
    }
}
