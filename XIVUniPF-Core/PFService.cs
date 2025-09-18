using System.Net.Http;
using System.Text;
using System.Text.Json;
using static XIVUniPF_Core.IPFService;

namespace XIVUniPF_Core
{
    public class PFService : IPFService
    {
        public struct Option
        {
            // 当前页码，默认为 1
            public int Page { get; set; }
            // 每页返回记录数，默认为 20，最大值 100
            public int PerPage { get; set; }
            // 按分类筛选
            public string Category { get; set; }
            // 按服务器筛选（匹配创建世界或主世界 e.g. 拉诺西亚）
            public string World { get; set; }
            // 关键字搜索，支持大小写不敏感匹配
            public string Search { get; set; }
            // 按数据中心筛选（e.g. 陆行鸟）
            public string Datacenter { get; set; }
            // 按职业 id 筛选，逗号分割
            public string Jobs { get; set; }
            // 按副本 id 筛选，逗号分割
            public string Duty { get; set; }

            public Option()
            {
                Page = 1;
                PerPage = 20;
                Category = string.Empty;
                World = string.Empty;
                Search = string.Empty;
                Datacenter = string.Empty;
                Jobs = string.Empty;
                Duty = string.Empty;
            }
        }

        // static
        private static readonly string Server = "https://xivpf.littlenightmare.top/";

        private static readonly string UserAgent = "XIVUniPF-Core 1.1 (contact: gfishfly@qq.com)";

        private static readonly JsonSerializerOptions defaultOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
        };

        // members
        public event PartyFinderUpdateEventHandler? OnPartyFinderUpdate;

        private readonly HttpClient clientProxy;

        private readonly HttpClient client;

        private PartyList parties;

        public PFService()
        {
            var noProxy = new HttpClientHandler()
            {
                UseProxy = false
            };
            client = new HttpClient(noProxy)
            {
                BaseAddress = new Uri(Server)
            };
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);

            clientProxy = new HttpClient()
            {
                BaseAddress = new Uri(Server)
            };
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);

            parties = new PartyList();
        }

        public PartyList GetParties()
        {
            return parties.Clone();
        }

        /// <summary>
        /// 获取全部招募数据
        /// 如果给服务器带来太大压力可能会改
        /// 更新后通过事件通知订阅方
        /// </summary>
        /// <param name="option">选项</param>
        /// <param name="progressCallback">进度更新时的回调函数 float为进度条增量</param>
        /// <returns></returns>
        public async Task Refresh(Action<float>? progressCallback = null, bool useProxy = true)
        {
            var option = new Option
            {
                Page = 1,
                PerPage = 100,
                Category = string.Empty,
                World = string.Empty,
                Search = string.Empty,
                Datacenter = string.Empty
            };

            // 先发一个请求，知道总共有多少页
            PartyList result = await Fetch(option, useProxy);
            var pages = result.Pagination.Total_pages;
            if (pages == 0)
            {
                parties = result;
                return;
            }
            progressCallback?.Invoke(100f / pages);

            // 请求剩下的页
            List<Task<PartyList>> tasks = [];
            for (int i = 2; i <= pages; i++)
            {
                option.Page = i;

                var task = Fetch(option, useProxy).ContinueWith(t =>
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

            parties = result;
            OnPartyFinderUpdate?.Invoke(this);
        }

        private async Task<PartyList> Fetch(Option option, bool useProxy)
        {
            var endpoint = new StringBuilder("/api/listings");
            try
            {
                endpoint.Append($"?page={option.Page}")
                          .Append($"&per_page={option.PerPage}");
                if (option.Category != string.Empty)
                    endpoint.Append($"&category={Uri.EscapeDataString(option.Category)}");
                if (option.Category != string.Empty)
                    endpoint.Append($"&world={Uri.EscapeDataString(option.World)}");
                if (option.Category != string.Empty)
                    endpoint.Append($"&search={Uri.EscapeDataString(option.Search)}");
                if (option.Category != string.Empty)
                    endpoint.Append($"&datacenter={Uri.EscapeDataString(option.Datacenter)}");
                if (option.Category != string.Empty)
                    endpoint.Append($"&jobs={Uri.EscapeDataString(option.Jobs)}");
                if (option.Category != string.Empty)
                    endpoint.Append($"&duty={Uri.EscapeDataString(option.Duty)}");

                var url = endpoint.ToString();
                using var response = useProxy switch
                {
                    true => await clientProxy.GetAsync(url),
                    false => await client.GetAsync(url),
                };
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
