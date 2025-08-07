using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using XIVUniPF.Classes;
using XIVUniPF.ViewModels;
using XIVUniPF_Core;

namespace XIVUniPF.Views
{
    public partial class MainWindow : Window
    {
        private static readonly SolidColorBrush localPartyTextColor = new(Color.FromRgb(0xE0, 0xA9, 0x33));

        private MainViewModel ViewModel => (MainViewModel)DataContext;

        // 是否正在输入法拼字阶段
        private bool _isComposing = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // 同步系统主题
            // 实验性 API
#pragma warning disable WPF0001
            Application.Current.ThemeMode = ThemeMode.System;
#pragma warning restore WPF0001

            PFService.Instance.OnPartyFinderUpdate += OnPartyFinderUpdate;

            var parties = PFService.Instance.GetParties();
            if (parties.Data.Count != 0)
            {
                ViewModel.Parties.Replace(parties.Data);
                ViewModel.IsLoading = false;
            }
            else
                FetchParties();
        }

        public async void FetchParties()
        {
            try
            {
                var option = new PFService.Option
                {
                    Page = 1,
                    PerPage = 100,
                    Category = string.Empty,
                    World = string.Empty,
                    Search = string.Empty,
                    Datacenter = string.Empty
                };
                ViewModel.IsLoading = true;
                ViewModel.LoadingProgress = 0;

                await PFService.Instance.Update(option,
                    delta => Dispatcher.Invoke(() => ViewModel.LoadingProgress += delta)
                );
            }
            catch (Exception)
            {
                ViewModel.IsLoading = false;
            }
        }

        public void OnPartyFinderUpdate(PFService sender)
        {
            var result = sender.GetParties();
            ViewModel.Parties.Replace(result.Data);
            ViewModel.IsLoading = false;
        }


        // 对本地招募标记为橙色
        // 用了一种比较 dirty 的实现
        // 如果能用 Binding + Converter实现会更好
        private void DutyName_Loaded(object sender, RoutedEventArgs e)
        {
            var s = (Wpf.Ui.Controls.TextBlock)sender;
            var context = (PartyInfo)s.DataContext;
            if (!context.Is_cross_world)
                s.Foreground = localPartyTextColor;
        }

        // 控件事件
        private void Refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            FetchParties();
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

        // 用于在 IME 拼字阶段屏蔽 SearchBox_TextChanged 事件，防止频繁刷新列表影响性能
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

        // F5 快捷键功能
        private void Refresh_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.Loaded;
            e.Handled = true;
        }

        private void Refresh_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FetchParties();
            e.Handled = true;
        }
    }
}