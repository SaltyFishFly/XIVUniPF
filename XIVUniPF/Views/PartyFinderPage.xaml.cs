using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using XIVUniPF.Classes;
using XIVUniPF.ViewModels;
using XIVUniPF_Core;

namespace XIVUniPF.Views
{
    public partial class PartyFinderPage : Page
    {
        private static readonly SolidColorBrush localPartyTextColor = new(Color.FromRgb(0xE0, 0xA9, 0x33));

        public event RoutedEventHandler? SettingsButtonClicked;

        private PartyFinderPageViewModel ViewModel => (PartyFinderPageViewModel)DataContext;

        // 是否正在输入法拼字阶段
        private bool _isComposing = false;

        public PartyFinderPage()
        {
            InitializeComponent();
            _ = ViewModel.GetParties();
        }

        // 对本地招募标记为橙色
        // 用了一种比较 dirty 的实现
        // 如果能用 Binding + Converter 实现会更好
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
            _ = ViewModel.Refresh();
        }

        private void Prev_Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Parties.Page--;
        }

        private void Next_Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Parties.Page++;
        }

        private void Settings_Button_Click(object sender, RoutedEventArgs e)
        {
            SettingsButtonClicked?.Invoke(this, e);
        }

        private void SortOption_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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
            _ = ViewModel.Refresh();
            e.Handled = true;
        }
    }
}
