using System.Windows;

using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using XIVUniPF.ViewModels;
using XIVUniPF_Core;

namespace XIVUniPF
{
    public partial class MainWindow : FluentWindow
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
            // 适应主题变化
            Loaded += (sender, args) => SystemThemeWatcher.Watch(this, WindowBackdropType.Mica, true);
            // 这里后面可以改
            var opt = new IPFDataSource.Options
            {
                Page = 1,
                PerPage = 100,
                Category = string.Empty,
                World = string.Empty,
                Search = string.Empty,
                Datacenter = string.Empty
            };
            LoadList(opt);
        }

        public async void LoadList(IPFDataSource.Options opt)
        {
            ViewModel.IsLoading = true;
            var res = await PFService.Instance.Fetch(opt);
            ViewModel.Parties.Replace(res.Data);
            ViewModel.Pagination = res.Pagination;
            ViewModel.IsLoading = false;
        }

        private void Refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            LoadList(ViewModel.Options.GetOptions());
        }

        private void Prev_Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Options.Page--;
            LoadList(ViewModel.Options.GetOptions());
        }

        private void Next_Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Options.Page++;
            LoadList(ViewModel.Options.GetOptions());
        }
    }
}