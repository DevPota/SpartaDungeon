namespace SpartaDungeon.Classes
{
    public enum ItemType
    {
        WEAPON,
        ARMOR,
        CONSUMABLE,
        MISC
    }

    public class Item
    {
        public string   Name      { get; protected set; }
        public string   InfoText  { get; protected set; }
        public ItemType Type      { get; protected set; }
        public int      Power     { get; protected set; }

        public Item(string name, string infoText, ItemType itemType, int itemPower)
        {
            Name = name;
            InfoText = infoText;
            Type  = itemType;
            Power = itemPower;
        }
    }

    public class Equipment : Item
    {
        public int enchanted  { get; private set; } = 0;
        public int durability { get; private set; } = 100;

        public Equipment(string name, string infoText, ItemType itemType, int power) : base(name, infoText, itemType, power)
        {
            if(itemType != ItemType.WEAPON && itemType != ItemType.ARMOR)
            {
                Type = ItemType.MISC;
            }
        }
    }

    public class Consumable : Item
    {
        public int count { get; private set; }

        public Consumable(string name, string infoText, int power) : base(name, infoText, ItemType.CONSUMABLE, power)
        {

        }
    }
}
