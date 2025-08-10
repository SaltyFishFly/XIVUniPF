using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Appearance;

namespace XIVUniPF.Views
{
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

        private void Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 0)
                return;

            switch (((ComboBox)sender).SelectedIndex)
            {
                case 0:
                    ApplicationThemeManager.Apply(ApplicationTheme.Light);
                    break;
                case 1:
                    ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                    break;
            }
        }
    }
}
