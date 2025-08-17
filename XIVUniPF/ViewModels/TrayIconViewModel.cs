using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using XIVUniPF.Classes;

namespace XIVUniPF.ViewModels
{
    public class TrayIconViewModel : INotifyPropertyChanged
    {
        public ICommand ShowWindowCommand
        {
            get => new BasicCommand(App.ShowMainWindow);
        }

        public ICommand ExitCommand
        {
            get => new BasicCommand(Application.Current.Shutdown);
        }

        // 实现接口
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
