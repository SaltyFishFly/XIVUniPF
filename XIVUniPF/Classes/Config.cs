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
        private bool _trayEnabled;

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

        public AppConfig()
        {
            _trayEnabled = true;
        }

        // 实现接口
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void Notify([CallerMemberName] string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
