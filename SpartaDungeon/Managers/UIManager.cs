using SpartaDungeon.Classes;
using SpartaDungeon.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpartaDungeon.Managers
{
    enum RightUI
    {
        CHARA_STATUS  = 0,
        INVENTORY     = 1,
        SHOP_PURCHASE = 2,
        SHOP_SELL     = 3,
        BLACKSMITH    = 4
    }

    static class UIManager
    {
        static GameCharacter characterData;
        public static RightUI RightUIStatus = RightUI.CHARA_STATUS;

        public static int        selected { get; set; } = 0;
        public static string[]   items = { "C0", "W0", "W1", "A0", "A1", "W2" };

        const char lt = '┌';
        const char rt = '┐';
        const char lb = '└';
        const char rb = '┘';
        const char tb = '─';
        const char s = '│';

        public static void Init(GameCharacter charData)
        {
            characterData = charData;
        }

        public static void RenderRightUI()
        {
            Console.SetCursorPosition(Define.UI_RIGHT_PIVOT, 0);
            Console.Write(lt);
            for (int i = 1; i < Define.UI_RIGHT_WIDTH - 1; i++)
            {
                Console.Write(tb);
            }
            Console.WriteLine(rt);

            for (int i = 1; i < Define.UI_RIGHT_HEIGHT - 1; i++)
            {
                Console.SetCursorPosition(Define.UI_RIGHT_PIVOT, i);
                Console.Write(s);
                for (int j = 1; j < Define.UI_RIGHT_WIDTH - 1; j++)
                {
                    Console.Write(' ');
                }
                Console.WriteLine(s);
            }

            Console.SetCursorPosition(Define.UI_RIGHT_PIVOT, Define.UI_RIGHT_HEIGHT - 1);
            Console.Write(lb);
            for (int i = 1; i < Define.UI_RIGHT_WIDTH - 1; i++)
            {
                Console.Write(tb);
            }
            Console.WriteLine(rb);

            switch(RightUIStatus)
            {
                case RightUI.CHARA_STATUS:
                    DisplayCharacterStatus();
                    break;
                case RightUI.INVENTORY:
                    DisplayInventory();
                    break;
                case RightUI.SHOP_PURCHASE:
                    DisplayPurchase();
                    break;
                case RightUI.SHOP_SELL:
                    DisplaySell();
                    break;
                case RightUI.BLACKSMITH:
                    break;
            }

            DisplayRightCmd();
        }

        static void DisplayRightCmd()
        {
            int pivotX = Define.UI_RIGHT_CMD_PIVOT_X;
            int pivotY = Define.UI_RIGHT_CMD_PIVOT_Y;

            Console.SetCursorPosition(pivotX, pivotY);

            switch(RightUIStatus)
            {
                case RightUI.CHARA_STATUS:
                    Console.Write("A.가방     B.종료");
                    break;
                case RightUI.INVENTORY:
                    Console.Write("A.사용     B.뒤로가기");
                    break;
                case RightUI.SHOP_PURCHASE:
                    Console.Write("A.구매     B.뒤로가기");
                    break;
                case RightUI.SHOP_SELL:
                    Console.Write("A.판매     B.뒤로가기");
                    break;
                case RightUI.BLACKSMITH:
                    Console.Write("A.강화     B.뒤로가기");
                    break;
            }
        }

        public static void RenderBottomUI()
        {
            Console.SetCursorPosition(0, Define.UI_BOTTOM_PIVOT);
            Console.Write(lt);
            for (int i = 1; i < Define.UI_BOTTOM_WIDTH - 1; i++)
            {
                Console.Write(tb);
            }
            Console.WriteLine(rt);

            for (int i = 1; i < Define.UI_BOTTOM_HEIGHT - 1; i++)
            {
                Console.SetCursorPosition(0, Define.UI_BOTTOM_PIVOT + i);
                Console.Write(s);
                for (int j = 1; j < Define.UI_BOTTOM_WIDTH - 1; j++)
                {
                    Console.Write(' ');
                }
                Console.WriteLine(s);
            }

            Console.SetCursorPosition(0, (Define.UI_BOTTOM_PIVOT + Define.UI_BOTTOM_HEIGHT) - 1);
            Console.Write(lb);
            for (int i = 1; i < Define.UI_BOTTOM_WIDTH - 1; i++)
            {
                Console.Write(tb);
            }
            Console.WriteLine(rb);
        }

        public static void RenderCmd(string[] eventNames)
        {
            Console.SetCursorPosition(0, Define.UI_CMD_PIVOT);
            Console.Write(' ');
            for(int i = 0; i < eventNames.Length; i++)
            {
                Console.Write("{0}.{1}", i+1, eventNames[i] + "      ");
            }
            Console.Write("\n 입력 >> ");
        }

        public static void DisplayBottomText(string name, string text)
        {
            int pivotX = 2;
            int pivotY = Define.UI_BOTTOM_PIVOT + 1;

            Console.SetCursorPosition(pivotX, pivotY);
            Console.Write(name);
            Console.SetCursorPosition(pivotX+2, pivotY+1);
            Console.Write(text);
        }

        static void DisplayCharacterStatus()
        {
            int pivotX = Define.UI_RIGHT_PIVOT + 2;
            int pivotY = 1;

            Console.SetCursorPosition((pivotX + Define.UI_RIGHT_WIDTH / 2)-6, pivotY);
            Console.Write("현재 상태");

            pivotY+=2;
            Console.SetCursorPosition(pivotX, pivotY);
            Console.Write("레벨   : {0}", characterData.Lv);

            pivotY+=2;
            Console.SetCursorPosition(pivotX, pivotY);
            Console.Write("경험치 : {0}/{1}", characterData.Exp , characterData.NextExp);

            pivotY+=2;
            Console.SetCursorPosition(pivotX, pivotY);
            if((characterData.MaxHP / 2) >= characterData.Hp)
            {
                Console.BackgroundColor = ConsoleColor.Red;
            }
            Console.Write("체력   : {0}/{1}", characterData.Hp, characterData.MaxHP);
            Console.ResetColor();

            pivotY+=2;
            Console.SetCursorPosition(pivotX, pivotY);
            int weaponDamage = characterData.GetWeaponDamage(characterData.GetCurrentWeaponIndex());
            Console.Write("개발력 : {0} + {1} ({2})", characterData.Str, weaponDamage, characterData.Str + weaponDamage);

            pivotY += 2;
            Console.SetCursorPosition(pivotX, pivotY);
            int armorPoint = characterData.GetArmorPoint(characterData.GetCurrentArmorIndex());
            Console.Write("근성   : {0} + {1} ({2})", characterData.Con, armorPoint, characterData.Con + armorPoint);

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            pivotY += 2;
            Console.SetCursorPosition(pivotX, pivotY);

            if (characterData.Weapon == "")
            {
                Console.Write("키보드 : 없음");
            }
            else
            {
                string[] weaponData = characterData.Weapon.Split('_');
                Console.Write("키보드 : {0}+{1} ({2}%)", Core.GetResource(weaponData[0]).Split(',')[1], weaponData[1], weaponData[2]);
            }

            Console.BackgroundColor = ConsoleColor.DarkGreen;
            pivotY += 2;
            Console.SetCursorPosition(pivotX, pivotY);
            
            if (characterData.Armor == "")
            {
                Console.Write("마우스 : 없음");
            }
            else
            {
                string[] armorData = characterData.Armor.Split('_');
                Console.Write("마우스 : {0}+{1} ({2}%)", Core.GetResource(armorData[0]).Split(',')[1], armorData[1], armorData[2]);
            }

            Console.ResetColor();

            pivotY +=4;
            Console.SetCursorPosition(pivotX, pivotY);
            Console.Write("스파르타 코인 : {0}", characterData.Golds);
        }

        static void DisplayInventory()
        {
            if (characterData.Inventory.Length <= selected || selected < 0)
            {
                selected = 0;
            }
            int pivotX = Define.UI_RIGHT_PIVOT - 1;
            int pivotY = 1;

            Console.SetCursorPosition(pivotX + (Define.UI_RIGHT_WIDTH / 2), pivotY);
            Console.Write("가방");

            int weaponEquipIndex = -1,
                armorEquipIndex  = -1;

            if(characterData.Weapon != "")
            {
                weaponEquipIndex = int.Parse(characterData.Weapon.Split('_')[3]);
            }
            if(characterData.Armor != "")
            {
                armorEquipIndex  = int.Parse(characterData.Armor.Split('_')[3]);
            }

            pivotX = Define.UI_RIGHT_PIVOT + 2;
            pivotY += 2;
            for (int i = 0; i < characterData.Inventory.Length; i++)
            {
                Console.SetCursorPosition(pivotX, pivotY);

                string[] splitedData = characterData.Inventory[i].Split('_');

                if (splitedData[0][0] == 'W' || splitedData[0][0] == 'A')
                {
                    if (weaponEquipIndex == i || armorEquipIndex == i)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("[E]");
                        Console.ResetColor();
                        Console.Write(" {0}.{1}+{2} ({3}%)", (char)('a' + i), Core.GetResource(splitedData[0]).Split(',')[1], splitedData[1], splitedData[2]);
                    }
                    else
                    {
                        Console.Write("    {0}.{1}+{2} ({3}%)", (char)('a' + i), Core.GetResource(splitedData[0]).Split(',')[1], splitedData[1], splitedData[2]);
                    }
                }
                else
                {
                    Console.Write("    {0}.{1} ({2})", (char)('a' + i), Core.GetResource(splitedData[0]).Split(',')[1], splitedData[1]);
                }

                if(i == selected)
                {
                    Console.SetCursorPosition(Define.UI_RIGHT_PIVOT + Define.UI_RIGHT_WIDTH - 4, pivotY);
                    Console.Write("<<");
                }
                pivotY++;

                Console.SetCursorPosition(pivotX, pivotY++);
                Console.Write("-------------------------------");
                
            }

            if(characterData.Inventory.Length == 0)
            {
                return;
            }
            else
            {
                int infoPivotX = Define.UI_RIGHT_INVENTORY_DIALOG_PIVOT_X;
                int infoPivotY = Define.UI_RIGHT_INVENTORY_DIALOG_PIVOT_Y;

                Console.SetCursorPosition(infoPivotX, infoPivotY);
                string[] selectedItemData = characterData.Inventory[selected].Split('_');
                string[] resourceData     = Core.GetResource(selectedItemData[0]).Split(',');

                string dialogText         = resourceData[2];

                for (int i = 0; i < dialogText.Length; i++)
                {
                    if (i % 18 == 0)
                    {
                        Console.SetCursorPosition(infoPivotX, ++infoPivotY);
                    }
                    Console.Write(dialogText[i]);
                }

                infoPivotX = Define.UI_RIGHT_CMD_PIVOT_X;
                infoPivotY = Define.UI_RIGHT_CMD_PIVOT_Y - 3;
                Console.SetCursorPosition(infoPivotX, infoPivotY++);

                ItemType type = (ItemType)int.Parse(resourceData[3]);

                Console.Write("타입 : ");

                switch(type)
                {
                    case ItemType.WEAPON:
                        Console.Write("키보드");
                        break;
                    case ItemType.ARMOR:
                        Console.Write("마우스");
                        break;
                    case ItemType.CONSUMABLE:
                        Console.Write("소모품");
                        break;
                }
                Console.SetCursorPosition(infoPivotX, infoPivotY);
                switch (type)
                {
                    case ItemType.WEAPON:
                        Console.Write("개발력 : {0}", characterData.GetWeaponDamage(selected));
                        break;
                    case ItemType.ARMOR:
                        Console.Write("근성 : {0}", characterData.GetArmorPoint(selected));
                        break;
                    case ItemType.CONSUMABLE:
                        Console.Write("회복 : {0}", resourceData[4]);
                        break;
                }
            }
        }

        static void DisplayPurchase()
        {
            if(selected < 0 || selected >= items.Length)
            {
                selected = 0;
            }
            int pivotX = Define.UI_RIGHT_PIVOT - 3;
            int pivotY = 1;

            Console.SetCursorPosition(pivotX + (Define.UI_RIGHT_WIDTH / 2), pivotY);
            Console.Write("구매하기");
            pivotX = Define.UI_RIGHT_PIVOT + 2;
            pivotY++;

            for (int i = 0; i < items.Length; i++)
            {
                pivotY++;
                Console.SetCursorPosition(pivotX, pivotY);

                string[] splitedData = Core.GetResource(items[i]).Split(',');

                ItemType type = (ItemType)int.Parse(splitedData[3]);

                switch(type)
                {
                    case ItemType.WEAPON:
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        break;
                    case ItemType.ARMOR:
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        break;
                    case ItemType.CONSUMABLE:
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        break;
                }

                Console.Write("{0}.{1}", (char)('a' + i), splitedData[1]);

                if (i == selected)
                {
                    Console.SetCursorPosition(Define.UI_RIGHT_PIVOT + Define.UI_RIGHT_WIDTH - 4, pivotY);
                    Console.Write("<<");
                }
                pivotY++;

                Console.ResetColor();

                Console.SetCursorPosition(pivotX, pivotY);
                Console.Write("-------------------------------");

            }

            int infoPivotX = Define.UI_RIGHT_INVENTORY_DIALOG_PIVOT_X;
            int infoPivotY = Define.UI_RIGHT_INVENTORY_DIALOG_PIVOT_Y - 5;

            Console.SetCursorPosition(infoPivotX, infoPivotY);
            string[] resourceData = Core.GetResource(items[selected]).Split(',');

            string dialogText = resourceData[2];

            for (int i = 0; i < dialogText.Length; i++)
            {
                if (i % 18 == 0)
                {
                    Console.SetCursorPosition(infoPivotX, ++infoPivotY);
                }
                Console.Write(dialogText[i]);
            }

            infoPivotX = Define.UI_RIGHT_CMD_PIVOT_X;
            infoPivotY = Define.UI_RIGHT_CMD_PIVOT_Y - 7;

            ItemType secletedType = (ItemType)int.Parse(resourceData[3]);

            if (int.Parse(resourceData[5]) > characterData.Golds)
            {
                Console.BackgroundColor = ConsoleColor.Red;
            }

            Console.SetCursorPosition(infoPivotX, infoPivotY++);
            Console.Write("가격   : {0}", resourceData[5]);
            Console.ResetColor();
            if (int.Parse(resourceData[5]) <= characterData.Golds)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
            }
            Console.SetCursorPosition(infoPivotX, infoPivotY++);
            Console.Write("소지금 : {0}", characterData.Golds);
            Console.ResetColor();
            infoPivotY++;
            Console.SetCursorPosition(infoPivotX, infoPivotY++);
            Console.Write("타입   : ");

            switch (secletedType)
            {
                case ItemType.WEAPON:
                    Console.Write("키보드");
                    break;
                case ItemType.ARMOR:
                    Console.Write("마우스");
                    break;
                case ItemType.CONSUMABLE:
                    Console.Write("소모품");
                    break;
            }

            Console.SetCursorPosition(infoPivotX, infoPivotY);
            switch (secletedType)
            {
                case ItemType.WEAPON:
                    Console.Write("개발력 : {0}", resourceData[4]);
                    break;
                case ItemType.ARMOR:
                    Console.Write("근성   : {0}", resourceData[4]);
                    break;
                case ItemType.CONSUMABLE:
                    Console.Write("회복   : {0}", resourceData[4]);
                    break;
            }
        }

        static void DisplaySell()
        {
            if (characterData.Inventory.Length == 0)
            {
                Core.PlaySFX(Define.SOUNDS_PATH + "/ShopNoItem.wav");
                RightUIStatus = RightUI.CHARA_STATUS;
                Core.SetSceneDialog("강영호 튜터", "팔 물건이 없네");
                selected = 0;
                DisplayCharacterStatus();

                return;
            }
            else if(characterData.Inventory.Length <= selected)
            {
                selected--;
            }
            int pivotX = Define.UI_RIGHT_PIVOT - 3;
            int pivotY = 1;

            Console.SetCursorPosition(pivotX + (Define.UI_RIGHT_WIDTH / 2), pivotY);
            Console.Write("판매하기");

            int weaponEquipIndex = -1,
                armorEquipIndex  = -1;

            if (characterData.Weapon != "")
            {
                weaponEquipIndex = int.Parse(characterData.Weapon.Split('_')[3]);
            }
            if (characterData.Armor != "")
            {
                armorEquipIndex  = int.Parse(characterData.Armor.Split('_')[3]);
            }

            pivotX = Define.UI_RIGHT_PIVOT + 2;
            pivotY += 2;

            for (int i = 0; i < characterData.Inventory.Length; i++)
            {
                Console.SetCursorPosition(pivotX, pivotY);

                string[] splitedData = characterData.Inventory[i].Split('_');

                if (splitedData[0][0] == 'W' || splitedData[0][0] == 'A')
                {
                    if (weaponEquipIndex == i || armorEquipIndex == i)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("[E]");
                        Console.ResetColor();
                        Console.Write(" {0}.{1}+{2} ({3}%)", (char)('a' + i), Core.GetResource(splitedData[0]).Split(',')[1], splitedData[1], splitedData[2]);
                    }
                    else
                    {
                        Console.Write("    {0}.{1}+{2} ({3}%)", (char)('a' + i), Core.GetResource(splitedData[0]).Split(',')[1], splitedData[1], splitedData[2]);
                    }
                }
                else
                {
                    Console.Write("    {0}.{1} ({2})", (char)('a' + i), Core.GetResource(splitedData[0]).Split(',')[1], splitedData[1]);
                }

                if (i == selected)
                {
                    Console.SetCursorPosition(Define.UI_RIGHT_PIVOT + Define.UI_RIGHT_WIDTH - 4, pivotY);
                    Console.Write("<<");
                }
                pivotY++;

                Console.SetCursorPosition(pivotX, pivotY++);
                Console.Write("-------------------------------");

            }

            if (characterData.Inventory.Length == 0)
            {
                return;
            }
            else
            {
                int infoPivotX = Define.UI_RIGHT_INVENTORY_DIALOG_PIVOT_X;
                int infoPivotY = Define.UI_RIGHT_INVENTORY_DIALOG_PIVOT_Y;

                Console.SetCursorPosition(infoPivotX, infoPivotY);
                string[] selectedItemData = characterData.Inventory[selected].Split('_');
                string[] resourceData = Core.GetResource(selectedItemData[0]).Split(',');

                string dialogText = resourceData[2];

                for (int i = 0; i < dialogText.Length; i++)
                {
                    if (i % 18 == 0)
                    {
                        Console.SetCursorPosition(infoPivotX, ++infoPivotY);
                    }
                    Console.Write(dialogText[i]);
                }

                infoPivotX = Define.UI_RIGHT_CMD_PIVOT_X;
                infoPivotY = Define.UI_RIGHT_CMD_PIVOT_Y - 3;
                Console.SetCursorPosition(infoPivotX, infoPivotY++);

                Console.Write("판매가격 : {0}", int.Parse(resourceData[5]) / 2);

                Console.SetCursorPosition(infoPivotX, infoPivotY);
                Console.Write("소지금   : {0}", Core.CharacterData.Golds);
            }
        }
    }
}