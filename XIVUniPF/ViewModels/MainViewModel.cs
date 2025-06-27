using System.ComponentModel;
using System.Runtime.CompilerServices;
using XIVUniPF.Classes;
using XIVUniPF_Core;

namespace XIVUniPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _isLoading;

        private ObservableOptions _options;

        private readonly DutyCollection _parties;

        private Pagination _pagination;


        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (value != _isLoading)
                {
                    _isLoading = value;
                    Notify();
                    Notify(nameof(Loaded));
                }
            }
        }

        public bool Loaded => !_isLoading;

        public ObservableOptions Options
        {
            get => _options;
            set
            {
                if (value != _options)
                {
                    _options = value;
                    Notify();
                }
            }
        }

        public string PageIndicator => $"{_pagination.Page} / {_pagination.Total_pages}";

        public DutyCollection Parties
        {
            get => _parties;
        }

        public Pagination Pagination
        {
            get => _pagination;
            set
            {
                if (value != _pagination)
                {
                    _pagination = value;
                    Notify();
                    Notify(nameof(PageIndicator));
                }
            }
        }

        public MainViewModel() 
        {
            IsLoading = false;
            _options = new ObservableOptions(new IPFDataSource.Options
            {
                Page = 1,
                PerPage = 50,
                Category = string.Empty,
                World = string.Empty,
                Search = string.Empty,
                Datacenter = string.Empty
            });
            _parties = [];
            _pagination = new Pagination();
        }

        // 实现接口
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
