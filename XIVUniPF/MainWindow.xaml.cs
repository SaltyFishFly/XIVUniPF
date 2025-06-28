using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using XIVUniPF.Classes;
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

            // Windows 11 下使用 Mica
            if (Environment.OSVersion.Version.Build >= 22000)
            {
                WindowBackdropType = WindowBackdropType.Mica;
                Loaded += (sender, args) => SystemThemeWatcher.Watch(this, WindowBackdropType.Mica, true);
            }
            else
            {
                WindowBackdropType = WindowBackdropType.Acrylic;
                Loaded += (sender, args) => SystemThemeWatcher.Watch(this, WindowBackdropType.Acrylic, true);
            }

            // 这里后面应该加入自定义
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
            try
            {
                ViewModel.IsLoading = true;
                var res = await PFService.Instance.Fetch(opt);
                ViewModel.Parties.Replace(res.Data);
                ViewModel.Pagination = res.Pagination;
                ViewModel.IsLoading = false;
            }
            catch (Exception e)
            {
                ViewModel.IsLoading = false;
            }
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

        private void SortOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var option = e.AddedItems.OfType<PartySortOption>().First();
            if (option == null)
                return;
            ViewModel.Parties.SortOption = option;
        }
    }
}