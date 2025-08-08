using System.Windows;
using XIVUniPF.ViewModels;

namespace XIVUniPF.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();

            NavigateToPartyFinderPage();
        }

        private void NavigateToPartyFinderPage()
        {
            var page = new PartyFinderPage();
            page.SettingsButtonClicked += PartyFinderPage_SettingsButtonClicked;
            MainFrame.Navigate(page);
        }

        private void NavigateToSettingsPage()
        {
            var settingsPage = new SettingsPage();
            MainFrame.Navigate(settingsPage);
            settingsPage.BackButtonClicked += SettingsPage_BackButtonClicked;
        }

        private void PartyFinderPage_SettingsButtonClicked(object sender, RoutedEventArgs e)
        {
            NavigateToSettingsPage();
        }

        private void SettingsPage_BackButtonClicked(object sender, RoutedEventArgs e)
        {
            NavigateToPartyFinderPage();
        }
    }
}