using SpartaDungeon.Common;
using SpartaDungeon.Classes;
using SpartaDungeon.Managers;
using System.Security.Cryptography.X509Certificates;

namespace SpartaDungeon
{
    public static class GameManager
    {
        public static void CheckGameOver()
        {
            if(Core.CharacterData.Hp < 0)
            {
                Core.GetEvent("EV_GameOver")();
                Core.CharacterData.Hp    = Core.CharacterData.MaxHP / 2;
                Core.CharacterData.Golds /= 2;

                Core.SaveData();
            }
        }

        public static void CheckLevelUp()
        {
            if(Core.CharacterData.Exp >= Core.CharacterData.NextExp)
            {
                Core.GetEvent("EV_LevelUp")();

                int levelUpCounts = Core.CharacterData.Exp / Core.CharacterData.NextExp;

                Core.CharacterData.Exp -= Core.CharacterData.NextExp;
                Core.CharacterData.NextExp += (Core.CharacterData.NextExp / 4);

                Core.CharacterData.Lv    += levelUpCounts;
                Core.CharacterData.MaxHP += 2 * levelUpCounts;
                Core.CharacterData.Str   += levelUpCounts;
                Core.CharacterData.Con   += levelUpCounts;
            }
        }

        public static void PlayDungeon(int level)
        {
            Core.PlaySFX(Define.SOUNDS_PATH + "/DungeonPlay.wav");

            Random random = new Random();

            int lowDefCut    = 10;
            int normalDefCut = 20;
            int highDefCut   = 40;

            int defCut = 0;
            int damage = random.Next(20, 35);
            int reward = 0;
            int exp    = 0;

            // Equipment damage
            int weaponDurabilityDamage = random.Next(1, 10);
            int armorDurabilityDamage  = random.Next(1, 10);
            
            if(Core.CharacterData.Weapon != "")
            {
                string[] weaponData = Core.CharacterData.Weapon.Split('_');
                int currentDurability = int.Parse(weaponData[2]) - weaponDurabilityDamage;
                if(currentDurability < 0)
                {
                    currentDurability = 0;
                }
                Core.CharacterData.Weapon = weaponData[0] + '_' + weaponData[1] + '_' + currentDurability + '_' + weaponData[3];
                Core.CharacterData.Inventory[int.Parse(weaponData[3])] = weaponData[0] + '_' + weaponData[1] + '_' + currentDurability;
            }

            if (Core.CharacterData.Armor != "")
            {
                string[] armorData = Core.CharacterData.Armor.Split('_');
                int currentDurability = int.Parse(armorData[2]) - armorDurabilityDamage;
                if (currentDurability < 0)
                {
                    currentDurability = 0;
                }
                Core.CharacterData.Armor = armorData[0] + '_' + armorData[1] + '_' + currentDurability + '_' + armorData[3];
                Core.CharacterData.Inventory[int.Parse(armorData[3])] = armorData[0] + '_' + armorData[1] + '_' + currentDurability;
            }

            switch (level)
            {
                case 0:
                    defCut = lowDefCut;
                    reward = 1000;
                    exp = 1;
                    break;
                case 1:
                    defCut = normalDefCut;
                    reward = 1700;
                    exp = 2;
                    break;
                case 2:
                    defCut = highDefCut;
                    reward = 2500;
                    exp = 3;
                    break;
            }
            int atk = Core.CharacterData.Str + Core.CharacterData.GetWeaponDamage(Core.CharacterData.GetCurrentWeaponIndex());
            int def = Core.CharacterData.Con + Core.CharacterData.GetArmorPoint(Core.CharacterData.GetCurrentArmorIndex());
            damage += defCut - def;
            reward += 2      * atk;

            int successRate = random.Next(0, 100);

            if (def >= defCut || successRate > 40)
            {
                Core.CharacterData.Hp    -= damage;
                Core.CharacterData.Golds += reward;
                Core.CharacterData.Exp   += exp;
                Core.SetSceneDialog("성공 하였습니다", "획득 코인 : " + reward + "  현재 체력(받은 피해) : " + Core.CharacterData.Hp + "(" + damage + ")" + "  획득 경험치 : " + exp);
            }
            else
            {
                Core.CharacterData.Hp -= damage / 2;
                Core.SetSceneDialog("실패 하였습니다", "획득 코인 : " + 0 + "  현재 체력(받은 피해) : " + Core.CharacterData.Hp + "(" + damage + ")" + "  획득 경험치 : " + 0);
            }

            Core.SaveData();
        }

        public static void Rest()
        {
            if (Core.CharacterData.Hp == Core.CharacterData.MaxHP)
            {
                Core.PlaySFX(Define.SOUNDS_PATH + "/RestFull.wav");
                Core.SetSceneDialog("이한별 튜터", "팔팔해보이는데?");
            }
            else if(Core.CharacterData.Golds >= 500)
            {
                Core.CharacterData.Golds -= 500;
                Core.CharacterData.Hp = Core.CharacterData.MaxHP;
                Core.PlaySFX(Define.SOUNDS_PATH + "/InnRest.wav");
                Core.SetSceneDialog("이한별 튜터", "편안한 밤 되시오 용사여 (500 코인 지불, 체력 100% 회복)");
            }
            else
            {
                Core.PlaySFX(Define.SOUNDS_PATH + "/InnNoCoin.wav");
                Core.SetSceneDialog("이한별 튜터", "코인이 부족해 (500 코인 필요)");
            }

            Core.SaveData();
        }

        public static void Purchase()
        {
            string[] itemData = Core.GetResource(UIManager.items[UIManager.selected]).Split(',');

            int price = int.Parse(itemData[5]);
            ItemType itemType = (ItemType)int.Parse(itemData[3]);
            
            if(Core.CharacterData.Golds >= price)
            {
                if(Core.CharacterData.AddItemToInventory(itemType, itemData[0]) == true)
                {
                    if (itemData[0] == "W2")
                    {
                        Core.PlaySFX(Define.SOUNDS_PATH + "/TILBuy.wav");
                        Core.SetSceneDialog("장윤서 매니저", "오늘 TIL 작성 하셨나요?");
                    }
                    else
                    {
                        Core.PlaySFX(Define.SOUNDS_PATH + "/ShopTrade.wav");
                        Core.SetSceneDialog("김영호 튜터", "고맙네");
                    }

                    Core.CharacterData.Golds -= price;
                }
                else
                {
                    Core.PlaySFX(Define.SOUNDS_PATH + "/ShopInventoryFull.wav");
                    Core.SetSceneDialog("김영호 튜터", "인벤토리가 꽉찼네");
                }
            }
            else
            {
                Core.PlaySFX(Define.SOUNDS_PATH + "/ShopNoCoin.wav");
                Core.SetSceneDialog("김영호 튜터", "코인이 부족하네");
            }
        }

        public static void Sell()
        {
            int sellPrice = int.Parse(Core.GetResource(Core.CharacterData.Inventory[UIManager.selected].Split('_')[0]).Split(',')[5]) / 2;
            bool isSold   = Core.CharacterData.RemoveItemFromInventory(UIManager.selected);

            if(isSold == true)
            {
                Core.PlaySFX(Define.SOUNDS_PATH + "/ShopTrade.wav");
                Core.SetSceneDialog("김영호 튜터", "고맙네");
                Core.CharacterData.Golds += sellPrice;
            }
        }

        public static void Repair()
        {
            if(Core.CharacterData.Golds >= 1000)
            {
                if (Core.CharacterData.Weapon == "" && Core.CharacterData.Armor == "")
                {
                    Core.PlaySFX(Define.SOUNDS_PATH + "/RepairNoEquipment.wav");
                    Core.SetSceneDialog("강인 튜터", "수리할 장비가 없군 (장착 장비가 없음)");
                    return;
                }

                string[] weaponData;
                string[] armorData;

                bool weaponRepaired = false;
                bool armorRepaired  = false;
                if (Core.CharacterData.Weapon != "")
                {
                    weaponData = Core.CharacterData.Weapon.Split('_');

                    if (int.Parse(weaponData[2]) != 100)
                    {
                        weaponRepaired = true;
                    }

                    Core.CharacterData.Weapon = weaponData[0] + '_' + weaponData[1] + '_' + 100 + '_' + weaponData[3];
                    Core.CharacterData.Inventory[int.Parse(weaponData[3])] = weaponData[0] + '_' + weaponData[1] + '_' + 100;

                }
                if (Core.CharacterData.Armor != "")
                {
                    armorData = Core.CharacterData.Armor.Split('_');

                    if (int.Parse(armorData[2]) != 100)
                    {
                        armorRepaired = true;
                    }

                    Core.CharacterData.Armor = armorData[0] + '_' + armorData[1] + '_' + 100 + '_' + armorData[3];
                    Core.CharacterData.Inventory[int.Parse(armorData[3])] = armorData[0] + '_' + armorData[1] + '_' + 100;

                }

                if(weaponRepaired == false && armorRepaired == false)
                {
                    Core.PlaySFX(Define.SOUNDS_PATH + "/RepairNoEquipment.wav");
                    Core.SetSceneDialog("강인 튜터", "수리할 장비가 없군 (장착 장비가 모두 내구도 100%)");
                }
                else
                {
                    Core.PlaySFX(Define.SOUNDS_PATH + "/RepairDone.wav");
                    Core.SetSceneDialog("강인 튜터", "수리가 끝났다 (1000 코인 소모)");

                    Core.CharacterData.Golds -= 1000;
                }
            }
            else
            {
                Core.PlaySFX(Define.SOUNDS_PATH + "/BlacksmithNoCoin.wav");
                Core.SetSceneDialog("강인 튜터", "코인이 부족하군 (1000 코인 필요)");
            }
        }

        public static void Enchant(ItemType equipmentType, string equipmentString)
        {
            if(equipmentString == "")
            {
                Core.PlaySFX(Define.SOUNDS_PATH + "/EnchantNoEquipment.wav");
                Core.SetSceneDialog("강인 튜터", "강화 할 장비를 장착해라 (장착 장비 없음)");
                return;
            }
            else if(Core.CharacterData.Golds < 5000)
            {
                Core.PlaySFX(Define.SOUNDS_PATH + "/BlacksmithNoCoin.wav");
                Core.SetSceneDialog("강인 튜터", "코인이 부족하군 (5000 코인 필요)");
                return;
            }

            string[] equipmentData = equipmentString.Split('_');
            int index = int.Parse(equipmentData[3]);

            string[] inventoryData = Core.CharacterData.Inventory[index].Split('_');

            int grade = int.Parse(equipmentData[1]);

            Random random = new Random();

            int minRange = 0;
            int maxRange = 100;
            int destroyCut = 2  + (grade / 10);
            int successCut = 20 + (grade - 10);

            int chance = random.Next(minRange, maxRange);

            if (grade % 5 == 0 || grade < 10 || chance >= successCut)
            {
                grade++;

                string newItem = inventoryData[0] + '_' + grade + '_' + inventoryData[2];
                Core.CharacterData.Inventory[index] = newItem;
                newItem += "_" + index;

                if(equipmentType == ItemType.WEAPON)
                {
                    Core.CharacterData.Weapon = newItem;
                }
                else if(equipmentType == ItemType.ARMOR)
                {
                    Core.CharacterData.Armor = newItem;
                }

                Core.PlaySFX(Define.SOUNDS_PATH + "/EnchantSuccess.wav");
                Core.SetSceneDialog("강인 튜터", "강화에 성공했다 (장비 등급 상승)");
            }
            else if(chance < successCut)
            {
                Core.PlaySFX(Define.SOUNDS_PATH + "/EnchantFail.wav");
                Core.SetSceneDialog("강인 튜터", "(안)미안하다 실패했다");
            }
            else if(chance <= destroyCut)
            {
                Core.CharacterData.RemoveItemFromInventory(index);
                Core.PlaySFX(Define.SOUNDS_PATH + "/EnchantFail.wav");
                Core.SetSceneDialog("강인 튜터", "(안)미안하다 실패했다 (장비 파괴)");
            }

            Core.CharacterData.Golds -= 5000;

            Core.SaveData();
        }
    }
}
