using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using XIVUniPF.Classes;
using XIVUniPF.ViewModels;
using XIVUniPF_Core;

namespace XIVUniPF.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => (MainViewModel)DataContext;

        // 是否正在输入法拼字阶段
        private bool _isComposing = false;

        public MainWindow()
        {
            InitializeComponent();

            // 同步系统主题
            // 实验性 API
#pragma warning disable WPF0001
            Application.Current.ThemeMode = ThemeMode.System;
#pragma warning restore WPF0001

            /*
            // 实验性：在 Windows10 上启用 Acrylic
            if (Environment.OSVersion.Version.Build < 22000)
            {
                Background = Brushes.Transparent;
                var comp = new WindowAccentCompositor(this);
                comp.Color = Color.FromArgb(200, 100, 100, 100);
                comp.IsEnabled = true;
            }
            */

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
                ViewModel.LoadingProgress = 0;

                var res = await PFService.Instance.FetchAll(
                    opt,
                    delta => Dispatcher.Invoke(() => ViewModel.LoadingProgress += delta)
                );
                ViewModel.Parties.Replace(res.Data);

                ViewModel.IsLoading = false;
            }
            catch (Exception)
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
            ViewModel.Parties.Page--;
        }

        private void Next_Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Parties.Page++;
        }

        private void SortOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var option = e.AddedItems.OfType<PartySortOption>().First();
            if (option == null)
                return;
            ViewModel.Parties.SortOption = option;
        }

        private void OnTextInputStart(object sender, TextCompositionEventArgs e)
        {
            _isComposing = true;
        }

        private void OnTextInputUpdate(object sender, TextCompositionEventArgs e)
        {
            _isComposing = true;
        }

        private void OnTextInputFinal(object sender, TextCompositionEventArgs e)
        {
            _isComposing = false;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isComposing) return;

            var textBox = sender as TextBox;
            // 手动将 TextBox 的 Text 推送到 ViewModel
            textBox?.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

            ViewModel.Parties.Update();
        }

        private void Refresh_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.Loaded;
            e.Handled = true;
        }

        private void Refresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LoadList(ViewModel.Options.GetOptions());
            e.Handled = true;
        }
    }
}