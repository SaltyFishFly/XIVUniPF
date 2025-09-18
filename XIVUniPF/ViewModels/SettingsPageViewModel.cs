using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XIVUniPF.ViewModels
{
    public class SettingsPageViewModel : INotifyPropertyChanged
    {
        // 实现接口
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
