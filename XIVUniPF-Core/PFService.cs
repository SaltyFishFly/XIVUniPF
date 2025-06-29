using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace XIVUniPF_Core
{
    public class PFService : IPFDataSource
    {
        private static readonly string BaseUrl = "https://xivpf.littlenightmare.top/";

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

        /// <summary>
        /// 获取全部招募数据
        /// 如果给服务器带来太大压力可能会改
        /// </summary>
        /// <param name="option">选项</param>
        /// <param name="progressCallback">进度更新时的回调函数 int为进度条增量</param>
        /// <returns></returns>
        public async Task<PartyList> FetchAll(IPFDataSource.Options option, Action<float>? progressCallback = null)
        {
            option.Page = 1;
            option.PerPage = 100;

            // 先发一个请求，知道总共有多少页
            PartyList result = await Fetch(option);
            var pages = result.Pagination.Total_pages;
            progressCallback?.Invoke(100f / pages);

            // 请求剩下的页
            List<Task<PartyList>> tasks = [];
            for (int i = 2; i <= pages; i++)
            {
                option.Page = i;

                var task = Fetch(option).ContinueWith(t =>
                {
                    progressCallback?.Invoke(100f / pages);
                    return t.Result;
                });

                tasks.Add(task);
            }

            // 合并结果
            await Task.WhenAll(tasks);

            result.Data.EnsureCapacity((pages - 1) * 100);
            foreach (var t in tasks)
                result.Data.AddRange(t.Result.Data);
            result.Pagination.Total_pages = 1;
            result.Pagination.Total = result.Data.Count;

            return result;
        }

        public async Task<PartyList> Fetch(IPFDataSource.Options option)
        {
            var urlBuilder = new StringBuilder(BaseUrl + "/api/listings");
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
