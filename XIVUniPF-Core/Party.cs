namespace XIVUniPF_Core
{
    public class PartyInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Created_world { get; set; }
        public int Created_world_id { get; set; }
        public string Home_world { get; set; }
        public int Home_world_id { get; set; }
        public string Category { get; set; }
        public int Category_id { get; set; }
        public string Duty { get; set; }
        public int Min_item_level { get; set; }
        public int Slots_filled { get; set; }
        public int Slots_available { get; set; }
        public float Time_left { get; set; }
        public DateTime Updated_at { get; set; }
        public bool Is_cross_world { get; set; }
        public string Datacenter { get; set; }

        public PartyInfo()
        {
            Id = 0;
            Name = string.Empty;
            Description = string.Empty;
            Created_world = string.Empty;
            Home_world = string.Empty;
            Category = string.Empty;
            Duty = string.Empty;
            Min_item_level = 0;
            Slots_filled = 0;
            Slots_available = 0;
            Time_left = 0.0f;
            Datacenter = string.Empty;
        }
    }

    public class Pagination
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int Per_page { get; set; }
        public int Total_pages { get; set; }
    }

    public class PartyList
    {
        public List<PartyInfo> Data { get; set; } = [];

        public Pagination Pagination { get; set; } = new();
    }
}
