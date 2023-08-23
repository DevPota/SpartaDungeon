using SpartaDungeon.Classes;
using SpartaDungeon.Managers;

namespace SpartaDungeon
{
    public class GameCharacter
    {
        public int Lv             { get; set; } = 1;
        public int Exp            { get; set; } = 0;
        public int NextExp        { get; set; } = 10;
        public int Hp             { get; set; } = 100;
        public int MaxHP          { get; set; } = 100;
        public int Str            { get; set; } = 10;
        public int Con            { get; set; } = 5;
        public int Golds          { get; set; } = 1000;
        public string Weapon      { get; set; } = "W0_0_100_0";
        public string Armor       { get; set; } = "A0_0_100_1";
        public string[] Inventory { get; set; } = new string[2] { "W0_0_100", "A0_0_100" };
        public int Location       { get; set; } = 1;

        public int GetWeaponDamage(int index)
        {
            if(index == -1)
            {
                return 0;
            }

            string[] weaponData = Inventory[index].Split('_');

            if (weaponData[2] == "0")
            {
                return 0;
            }
            else
            {
                return int.Parse(Core.GetResource(weaponData[0]).Split(',')[4]) + int.Parse(weaponData[1]);
            }
        }

        public int GetArmorPoint(int index)
        {
            if (index == -1)
            {
                return 0;
            }

            string[] armorData = Inventory[index].Split('_');

            if (armorData[2] == "0")
            {
                return 0;
            }
            else
            {
                return int.Parse(Core.GetResource(armorData[0]).Split(',')[4]) + int.Parse(armorData[1]);
            }
        }

        public int GetCurrentWeaponIndex()
        {
            if(Weapon == "")
            {
                return -1;
            }
            else
            {
                string[] weaponData = Weapon.Split('_');

                return int.Parse(weaponData[3]);
            }
        }

        public int GetCurrentArmorIndex()
        {
            if (Armor == "")
            {
                return -1;
            }
            else
            {
                string[] armorData = Armor.Split('_');

                return int.Parse(armorData[3]);
            }
        }

        public int GetItemIndex(string itemKey)
        {
            for(int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i].Split('_')[0] == itemKey)
                {
                    return i;
                }
            }
            
            return -1;
        }

        public void Equip(int index)
        {
            string[] itemData = Inventory[index].Split('_');

            switch (itemData[0][0])
            {
                case 'W' :
                    if (Weapon != "")
                    {
                        if (int.Parse(Weapon.Split('_')[3]) == index)
                        {
                            Weapon = "";
                        }
                        else
                        {
                            Weapon = Inventory[index] + '_' + index;
                        }
                    }
                    else
                    {
                        Weapon = Inventory[index] + '_' + index;
                    }
                    break;
                case 'A' :
                    if (Armor != "")
                    {
                        if (int.Parse(Armor.Split('_')[3]) == index)
                        {
                            Armor = "";
                        }
                        else
                        {
                            Armor = Inventory[index] + '_' + index;
                        }
                    }
                    else
                    {
                        Armor = Inventory[index] + '_' + index;
                    }
                    break;
                case 'C' :
                    break;
            }


        }

        public bool IsInventoryEmpty()
        {
            if(Inventory.Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetItemFromInventory(string itemKey)
        {
            foreach(string item in Inventory)
            {
                if(item.Split('_')[0] == itemKey == true)
                {
                    return item;
                }
            }

            return "";
        }

        public bool AddItemToInventory(ItemType type, string itemKey)
        {
            if(Inventory.Length == 10)
            {
                return false;
            }

            if(ItemType.WEAPON == type || ItemType.ARMOR == type)
            {
                itemKey += "_0_100";
                string[] newInventory = new string[Inventory.Length + 1];

                for(int i = 0; i < Inventory.Length; i++)
                {
                    newInventory[i] = Inventory[i];
                }
                newInventory[Inventory.Length] = itemKey;
                Inventory = newInventory;
            }
            else
            {
                string item = GetItemFromInventory(itemKey);

                if (item == "")
                {
                    itemKey += "_1";

                    string[] newInventory = new string[Inventory.Length + 1];

                    for (int i = 0; i < Inventory.Length; i++)
                    {
                        newInventory[i] = Inventory[i];
                    }
                    newInventory[Inventory.Length] = itemKey;
                    Inventory = newInventory;
                }
                else
                {
                    int itemIndex = GetItemIndex(itemKey);
                    itemKey += "_" + (int.Parse(item.Split('_')[1]) + 1);

                    if (itemIndex == -1)
                    {
                        return false;
                    }
                    else
                    {
                        Inventory[itemIndex] = itemKey;
                    }
                }
            }

            Core.SaveData();

            return true;
        }

        public bool RemoveItemFromInventory(int index)
        {
            if(Inventory.Length == 0)
            {
                return false;
            }

            string[] splitedItemData = GetItemFromInventory(Inventory[index].Split('_')[0]).Split('_');
            string[] ResourceData = Core.GetResource(splitedItemData[0]).Split(',');
            ItemType type = (ItemType)int.Parse(ResourceData[3]);

            if (type == ItemType.WEAPON || type == ItemType.ARMOR)
            {
                string[] newInventory = new string[Inventory.Length - 1];
                int idx = 0;

                if (Weapon != "" && type == ItemType.WEAPON)
                {
                    if (int.Parse(Weapon.Split('_')[3]) == index)
                    {
                        Weapon = "";
                    }

                    if(Armor != "")
                    {
                        string[] armorData = Armor.Split('_');
                        int armorIndex = int.Parse(armorData[3]);

                        if(index < armorIndex)
                        {
                            armorIndex--;
                        }

                        Armor = armorData[0] + '_' + armorData[1] + '_' + armorData[2] + '_' + armorIndex.ToString();
                    }
                }
                else if (Armor != "" && type == ItemType.ARMOR)
                {
                    if (int.Parse(Armor.Split('_')[3]) == index)
                    {
                        Armor = "";
                    }

                    if (Weapon != "")
                    {
                        string[] weaponData = Weapon.Split('_');
                        int weaponIndex = int.Parse(weaponData[3]);

                        if (index < weaponIndex)
                        {
                            weaponIndex--;
                        }

                        Weapon = weaponData[0] + '_' + weaponData[1] + '_' + weaponData[2] + '_' + weaponIndex.ToString();
                    }
                }

                for (int i = 0; i < Inventory.Length; i++)
                {
                    if (i == index)
                    {
                        continue;
                    }
                    else
                    {
                        newInventory[idx] = Inventory[i];
                        idx++;
                    }
                }

                Inventory = newInventory;
            }
            else
            {
                int itemCount = int.Parse(splitedItemData[1]) - 1;

                if(itemCount <= 0)
                {
                    if(Weapon != "")
                    {
                        string[] weaponData = Weapon.Split('_');
                        int      weaponIndex = int.Parse(weaponData[3]);

                        if (index < weaponIndex)
                        {
                            weaponIndex--;
                        }

                        Weapon = weaponData[0] + '_' + weaponData[1] + '_' + weaponData[2] + '_' + weaponIndex.ToString();
                    }
                    if (Armor != "")
                    {
                        string[] armorData = Armor.Split('_');
                        int armorIndex = int.Parse(armorData[3]);

                        if (index < armorIndex)
                        {
                            armorIndex--;
                        }
                        Armor = armorData[0] + '_' + armorData[1] + '_' + armorData[2] + '_' + armorIndex.ToString();
                    }

                    UIManager.selected--;
                    string[] newInventory = new string[Inventory.Length - 1];
                    int idx = 0;
                    for (int i = 0; i < Inventory.Length; i++)
                    {
                        if (i == index)
                        {
                            continue;
                        }
                        else
                        {
                            newInventory[idx] = Inventory[i];
                            idx++;
                        }
                    }

                    Inventory = newInventory;
                }
                else
                {
                    splitedItemData[1] = "" + itemCount;

                    string countedItem = splitedItemData[0] + '_' + splitedItemData[1];

                    Inventory[index] = countedItem;
                }
            }

            Core.SaveData();

            return true;
        }

        public void UseConsumable(int index)
        {
            Hp += int.Parse(Core.GetResource(Inventory[index].Split('_')[0]).Split(',')[4]);

            if(Hp > MaxHP)
            {
                Hp = MaxHP;
            }

            RemoveItemFromInventory(index);
        }
    }
}
