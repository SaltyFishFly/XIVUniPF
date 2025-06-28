using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace XIVUniPF_Core
{
    public class PFService : IPFDataSource
    {
        public static PFService Instance => _instance.Value;

        // 懒加载
        private static readonly Lazy<PFService> _instance = new(() => new PFService());

        private static readonly JsonSerializerOptions defaultOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
        };

        private readonly HttpClient client = new();

        public PFService()
        {
            client.DefaultRequestHeaders.Add("User-Agent", "XIVUniPF-Core 1.0 (contact: gfishfly@qq.com)");
        }

        public async Task<PartyList> Fetch(IPFDataSource.Options option)
        {
            var urlBuilder = new StringBuilder("https://xivpf.littlenightmare.top/api/listings");
            try
            {
                urlBuilder.Append($"?page={option.Page}")
                          .Append($"&per_page={option.PerPage}");
                if (option.Category != string.Empty)
                    urlBuilder.Append($"&category={Uri.EscapeDataString(option.Category)}");
                if (option.Category != string.Empty)
                    urlBuilder.Append($"&world={Uri.EscapeDataString(option.World)}");
                if (option.Category != string.Empty)
                     urlBuilder.Append($"&search={Uri.EscapeDataString(option.Search)}");
                if (option.Category != string.Empty)
                     urlBuilder.Append($"&datacenter={Uri.EscapeDataString(option.Datacenter)}");
                if (option.Category != string.Empty)
                    urlBuilder.Append($"&jobs={Uri.EscapeDataString(option.Jobs)}");
                if (option.Category != string.Empty)
                    urlBuilder.Append($"&duty={Uri.EscapeDataString(option.Duty)}");

                var url = urlBuilder.ToString();
                using var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                return
                    JsonSerializer.Deserialize<PartyList>(json, defaultOptions)
                    ?? throw new JsonException("反序列化结果为null");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"请求失败: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"JSON 解析失败: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"未知错误: {ex.Message}", ex);
            }
        }
    }
}
