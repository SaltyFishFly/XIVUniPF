using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using XIVUniPF.Classes;
using XIVUniPF.Views;

namespace XIVUniPF.ViewModels
{
    public class TrayIconViewModel : INotifyPropertyChanged
    {
        public ICommand ShowWindowCommand
        {
            get => new BasicCommand(() =>
                {
                    var window = Application.Current.MainWindow;
                    if (window == null)
                    {
                        window = Application.Current.MainWindow = new MainWindow();
                        window.Show();
                        return;
                    }
                    if (window.WindowState == WindowState.Minimized)
                        window.WindowState = WindowState.Normal;
                    window.Activate();
                }
            );
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
