using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XIVUniPF.ViewModels
{
    public partial class TrayIconViewModel : INotifyPropertyChanged
    {

        [RelayCommand]
        public void ShowMainWindow()
        {
            App.ShowMainWindow();
        }

        [RelayCommand]
        public void Exit()
        {
            App.Current.Shutdown();
        }

        // 实现接口
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
