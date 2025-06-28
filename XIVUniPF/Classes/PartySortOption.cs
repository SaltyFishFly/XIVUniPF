using XIVUniPF_Core;

namespace XIVUniPF.Classes
{
    public class PartySortOption
    {
        public string Name { get; set; }

        public Func<PartyInfo, PartyInfo, int> Comparison { get; set; }

        public PartySortOption(string name, Func<PartyInfo, PartyInfo, int> predicate)
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
            new("任务分类", (a, b) => a.Category_id < b.Category_id ? -1 : 1);
    }
}
