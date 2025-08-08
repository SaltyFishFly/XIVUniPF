using System.Windows;
using System.Windows.Controls;

namespace XIVUniPF.Views
{
    /// <summary>
    /// SettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsPage : Page
    {
        public event RoutedEventHandler? BackButtonClicked;

        public SettingsPage()
        {
            InitializeComponent();
        }

        private void Settings_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BackButtonClicked?.Invoke(this, e);
        }
    }
}
