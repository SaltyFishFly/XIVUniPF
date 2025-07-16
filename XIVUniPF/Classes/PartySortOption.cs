using XIVUniPF_Core;

namespace XIVUniPF.Classes
{
    public class PartySortOption
    {
        public string Name { get; set; }

        // Comparison 返回 -1 则 a 在前面，反之 b 在前面
        public Comparison<PartyInfo> Comparison { get; set; }

        public PartySortOption(string name, Comparison<PartyInfo> predicate)
        {
            Name = name;
            Comparison = predicate;
        }
    }

    public static class PartySortOptions
    {
        public static readonly PartySortOption TimeLeft =
            new("剩余时间", (a, b) => a.Time_left < b.Time_left ? -1 : 1);

        public static readonly PartySortOption TimeLeftReversed =
            new("剩余时间（反向）", (a, b) => -TimeLeft.Comparison(a, b));

        public static readonly PartySortOption Category =
            new("任务分类", (a, b) =>
            {
                if (a.Category_id > b.Category_id)
                    return -1;
                if (a.Category_id < b.Category_id)
                    return 1;
                // 相同时 fallback 到时间排序
                return a.Time_left < b.Time_left ? -1 : 1;
            });

        public static readonly PartySortOption CategoryReversed =
            new("任务分类（反向）", (a, b) => -Category.Comparison(a, b));

        public static readonly PartySortOption Datacenter =
            new("所在大区", (a, b) =>
            {
                var aId = Utils.getDatacenterId(a.Datacenter);
                var bId = Utils.getDatacenterId(b.Datacenter);
                if (aId < bId)
                    return -1;
                if (aId > bId)
                    return 1;
                return a.Time_left < b.Time_left ? -1 : 1;
            });

        public static readonly PartySortOption DatacenterReversed =
            new("所在大区（反向）", (a, b) => -Datacenter.Comparison(a, b));
    }
}
