namespace XIVUniPF_Core
{
    public interface IPFDataSource
    {
        public class Options
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

            public Options()
            {
                Page = 1;
                PerPage = 20;
                Category = string.Empty;
                World = string.Empty;
                Search = string.Empty;
                Datacenter = string.Empty;
            }
        }

        public Task<PartyList> Fetch(Options op);
    }
}
