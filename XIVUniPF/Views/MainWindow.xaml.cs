using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Wpf.Ui.Controls;
using XIVUniPF.ViewModels;

namespace XIVUniPF.Views
{
    public partial class MainWindow : FluentWindow
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(Resources["PartyFinderPage"]);
        }

        private void SettingsPage_BackButtonClicked(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(Resources["PartyFinderPage"]);
        }

        private void PartyFinderPage_SettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(Resources["SettingsPage"]);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (!App.Config.TrayEnabled)
                App.Current.Shutdown();
        }

        // navigate 动画
        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Content is Page page && page.Content is FrameworkElement root)
            {
                // 起始条件
                root.Opacity = 0;
                root.Margin = new Thickness(0, 100, 0, -100);

                // 淡入&上移动画
                var fadeIn = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                var slideUp = new ThicknessAnimation
                {
                    From = new Thickness(0, 50, 0, -50),
                    To = new Thickness(0),
                    Duration = TimeSpan.FromMilliseconds(200),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                root.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                root.BeginAnimation(FrameworkElement.MarginProperty, slideUp);
            }
        }
    }
}