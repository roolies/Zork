namespace Zork.Common
{
    public class Item
    {
        public string Name { get; }

        public string LookDescription { get; }

        public string InventoryDescription { get; }

        public bool IsEdible { get; }

        public Item(string name, string lookDescription, string inventoryDescription, bool isEdible)
        {
            Name = name;
            LookDescription = lookDescription;
            InventoryDescription = inventoryDescription;
            IsEdible = isEdible;
        }
        public override string ToString() => Name;
    }
}