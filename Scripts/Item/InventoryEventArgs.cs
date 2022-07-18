namespace Mdfry1.Scripts.Item
{
    public class InventoryEventArgs
    {
        public int Amount { get; set; } = 1;
        public Item Item { get; set; }

        public InventoryEventArgs(Item item) => Item = item;

        public InventoryEventArgs(Item item, int amt)
        {
            Item = item;
            Amount = amt;
        }
    }
}