namespace XIVUniPF.Classes
{
    public static class Utils
    {
        // 获取的 id 是我瞎编的
        // 为什么猪区在猫区前面，因为作者是猪区人（?）
        public static int getDatacenterId(string name)
        {
            return name switch
            {
                "陆行鸟" => 1,
                "莫古力" => 2,
                "猫小胖" => 3,
                "豆豆柴" => 4,
                _ => 5
            };
        }
    }
}
