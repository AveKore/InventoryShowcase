namespace _Project.Scripts
{
    public class ItemData
    {
        public int CurrentCount { get; set; }
        public ItemConfig ItemConfig { get; }

        public ItemData(ItemConfig config)
        {
            ItemConfig = config;
            CurrentCount = ItemConfig.IsStackable ? ItemConfig.Count : 1;
        }
    }
}