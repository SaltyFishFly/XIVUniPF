using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using XIVUniPF.Classes;
using XIVUniPF.Classes.Filters;

namespace XIVUniPF.ViewModels
{
    public class PartyFinderPageViewModel : INotifyPropertyChanged
    {
        private bool _isLoading;

        private float _loadingProgress;

        private readonly PartyCollection _parties;

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

        public PartyFinderPageViewModel()
        {
            // init
            IsLoading = true;
            LoadingProgress = 0;
            _parties = [];
            _sortOptions = new ObservableCollection<PartySortOption>(
                from field in typeof(PartySortOptions).GetFields(BindingFlags.Public | BindingFlags.Static)
                let option = field.GetValue(null)!
                where option != null
                select (PartySortOption)option
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
