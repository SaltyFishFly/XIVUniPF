using XIVUniPF_Core;

namespace XIVUniPF.Classes
{
    public class PartySortOption
    {
        public string Name { get; set; }

        public Comparison<PartyInfo> Comparison { get; set; }

        public PartySortOption(string name, Comparison<PartyInfo> predicate)
        {
            Name = name;
            Comparison = predicate;
        }
    } 

    public class PartySortOptions
    {
        public static readonly PartySortOption TimeLeft =
            new("剩余时间", (a, b) => a.Time_left < b.Time_left ? -1 : 1);

        public static readonly PartySortOption Category =
            new("任务分类", (a, b) => a.Category_id > b.Category_id ? -1 : 1);

        public static readonly PartySortOption Datacenter =
            new("大区", (a, b) =>
            {
                var aId = Utils.getDatacenterId(a.Datacenter);
                var bId = Utils.getDatacenterId(b.Datacenter);
                if (aId > bId)
                    return 1;
                if (aId < bId)
                    return -1;
                return a.Created_world_id > b.Created_world_id ? -1 : 1;
            });
    }
}
