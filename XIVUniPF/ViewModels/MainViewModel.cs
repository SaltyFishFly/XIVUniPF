using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using XIVUniPF.Classes;
using XIVUniPF.Classes.Filters;
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

        private SearchBoxFilter _searchBoxFilter;


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

        public bool IsPrevButtonEnabled => Loaded && _parties.Page > 1;

        public bool IsNextButtonEnabled => Loaded && _parties.Page < _parties.PageCount;

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

        public string PageIndicator => $"{_parties.Page} / {_parties.PageCount}";

        public PartyCollection Parties
        {
            get => _parties;
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
            _sortOptions = new ObservableCollection<PartySortOption>(
                typeof(PartySortOptions)
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Select(f => (PartySortOption)f.GetValue(null)!)
                    .Where(v => v != null)
            );
            _keywords = string.Empty;
            _searchBoxFilter = new SearchBoxFilter();

            // 添加关键词过滤器
            _parties.AddFilter(info => _searchBoxFilter.Predict(info, _keywords));

            _parties.PageChanged += (sender) =>
            {
                Notify(nameof(PageIndicator));
                Notify(nameof(IsPrevButtonEnabled));
                Notify(nameof(IsNextButtonEnabled));
            };
        }

        // 实现接口
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
