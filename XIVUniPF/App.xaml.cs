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
        }
    }
}
