using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace XIVUniPF.Classes
{
    public class YamlConfigManager<T> where T : new()
    {
        private readonly string path;
        private readonly ISerializer serializer;
        private readonly IDeserializer deserializer;

        public YamlConfigManager(string filePath)
        {
            path = filePath;
            serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .Build();
            deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();
        }

        public T Load()
        {
            if (!File.Exists(path))
            {
                var defaultConfig = new T();
                Save(defaultConfig);
                return defaultConfig;
            }

            var yaml = File.ReadAllText(path);
            return deserializer.Deserialize<T>(yaml);
        }

        public void Save(T config)
        {
            var yaml = serializer.Serialize(config);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, yaml);
        }
    }

    public class AppConfig : INotifyPropertyChanged
    {
        private bool _trayEnabled = true;
        public bool TrayEnabled
        {
            get => _trayEnabled;
            set
            {
                if (_trayEnabled != value)
                {
                    _trayEnabled = value;
                    Notify();
                }
            }
        }

        private bool _useSystemProxy = true;
        public bool UseSystemProxy
        {
            get => _useSystemProxy;
            set
            {
                if (_useSystemProxy != value)
                {
                    _useSystemProxy = value;
                    Notify();
                }
            }
        }

        private bool _autoCheckUpdates = true;
        public bool AutoCheckUpdates
        {
            get => _autoCheckUpdates;
            set
            {
                if (_autoCheckUpdates != value)
                {
                    _autoCheckUpdates = value;
                    Notify();
                }
            }
        }

        private bool _autoRefresh = false;
        public bool AutoRefresh
        {
            get => _autoRefresh;
            set
            {
                if (_autoRefresh != value)
                {
                    _autoRefresh = value;
                    Notify();
                }
            }
        }

        private double _autoRefreshInverval = 30;
        public double AutoRefreshInterval
        {
            get => _autoRefreshInverval;
            set
            {
                if (_autoRefreshInverval != value)
                {
                    _autoRefreshInverval = value;
                    Notify();
                }
            }
        }

        // 实现接口
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
