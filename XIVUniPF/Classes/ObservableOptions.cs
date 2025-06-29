using System.ComponentModel;
using System.Runtime.CompilerServices;
using static XIVUniPF_Core.IPFDataSource;

namespace XIVUniPF.Classes
{
    public class ObservableOptions : INotifyPropertyChanged
    {
        private Options _options;

        public ObservableOptions()
        {
            _options = new Options();
        }

        public ObservableOptions(Options options)
        {
            _options = options;
        }

        public int Page
        {
            get => _options.Page;
            set
            {
                if (_options.Page != value)
                {
                    _options.Page = value;
                    Notify();
                }
            }
        }

        public int PerPage
        {
            get => _options.PerPage;
            set
            {
                if (_options.PerPage != value)
                {
                    _options.PerPage = value;
                    Notify();
                }
            }
        }

        public string Category
        {
            get => _options.Category;
            set
            {
                if (_options.Category != value)
                {
                    _options.Category = value;
                    Notify();
                }
            }
        }

        public string World
        {
            get => _options.World;
            set
            {
                if (_options.World != value)
                {
                    _options.World = value;
                    Notify();
                }
            }
        }

        public string Search
        {
            get => _options.Search;
            set
            {
                if (_options.Search != value)
                {
                    _options.Search = value;
                    Notify();
                }
            }
        }

        public string Datacenter
        {
            get => _options.Datacenter;
            set
            {
                if (_options.Datacenter != value)
                {
                    _options.Datacenter = value;
                    Notify();
                }
            }
        }

        public Options GetOptions()
        {
            return _options;
        }

        // 实现接口
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void Notify([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
