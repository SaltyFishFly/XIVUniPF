using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using XIVUniPF.Classes;
using XIVUniPF_Core;

namespace XIVUniPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _isLoading;

        private float _loadingProgress;

        private ObservableOptions _options;

        private readonly PartyCollection _parties;

        private Pagination _pagination;

        private readonly ObservableCollection<PartySortOption> _sortOptions;

        private String _keywords;


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
                    Notify(nameof(IsPrevButtonEnabled));
                    Notify(nameof(IsNextButtonEnabled));
                }
            }
        }

        public bool Loaded => !_isLoading;

        public float LoadingProgress
        {
            get => _loadingProgress;
            set
            {
                if (value != _loadingProgress)
                {
                    _loadingProgress = value;
                    Notify();
                }
            }
        }

        public bool IsPrevButtonEnabled => Loaded && _pagination.Page > 1;

        public bool IsNextButtonEnabled => Loaded && _pagination.Page < _pagination.Total_pages;

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

        public PartyCollection Parties
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
                    Notify(nameof(IsPrevButtonEnabled));
                    Notify(nameof(IsNextButtonEnabled));
                }
            }
        }

        public ObservableCollection<PartySortOption> SortOptions => _sortOptions;

        public String Keywords
        {
            get => _keywords;
            set
            {
                if (value != _keywords)
                {
                    _keywords = value;
                    Notify();
                }
            }
        }

        public MainViewModel() 
        {
            // init
            IsLoading = true;
            LoadingProgress = 0;
            _options = new ObservableOptions(new IPFDataSource.Options
            {
                Page = 1,
                PerPage = 100,
                Category = string.Empty,
                World = string.Empty,
                Search = string.Empty,
                Datacenter = string.Empty
            });
            _parties = [];
            _pagination = new Pagination();
            _sortOptions =
            [
                PartySortOptions.TimeLeft,
                PartySortOptions.Category,
                PartySortOptions.Datacenter,
            ];
            _keywords = string.Empty;

            // 添加关键词过滤器
            _parties.AddFilter(info =>
            {
                var bindViewmodel = this;
                var keywords = bindViewmodel.Keywords.Trim();
                if (keywords == string.Empty)
                    return true;

                if (info.Description.Contains(keywords))
                    return true;

                return false;
            });
        }

        // 实现接口
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
