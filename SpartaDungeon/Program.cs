using System.Text;
using SpartaDungeon.Managers;
using SpartaDungeon.Common;
using SpartaDungeon.Classes;

namespace SpartaDungeon
{
    public static class Program
    {
        public static bool applicationQuit { get; set; } = false;

        static void Main(string[] args)
        {
            Awake();
            Start();

            while (applicationQuit == false)
            {
                Update();
            }
        }

        static void Awake()
        {
            Console.InputEncoding  = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            Core.Init();
            UIManager.Init(Core.CharacterData);
        }

        static void Start()
        {
            Core.GetEvent("EV_Menu")();
        }

        static void Update()
        {
            GameManager.CheckLevelUp();
            GameManager.CheckGameOver();

            Console.Clear();
            Console.SetWindowSize(Define.SCREEN_WIDTH, Define.SCREEN_HEIGHT);
            Core.Render();
            UIManager.RenderRightUI();
            UIManager.RenderBottomUI();
            UIManager.DisplayBottomText(Core.GetDialogData()[0], Core.GetDialogData()[1]);
            UIManager.RenderCmd(Core.GetCurrentScene().SceneActionKeys);
            char ipt = Core.GetInput();

            if(ipt == -1)
            {

            }
            else if(ipt - '0' <= Core.GetCmdRange() && ipt - '0' > 0)
            {
                Core.GetEvent(ipt - '0' - 1).Invoke();
            }
            else if(ipt == 'A')
            {
                switch (UIManager.RightUIStatus)
                {
                    case RightUI.CHARA_STATUS:
                        UIManager.selected = 0;
                        UIManager.RightUIStatus = RightUI.INVENTORY;
                        break;
                    case RightUI.INVENTORY:
                        if(Core.CharacterData.IsInventoryEmpty() == false)
                        {
                            if (Core.CharacterData.Inventory[UIManager.selected][0] == 'C')
                            {
                                Core.PlaySFX(Define.SOUNDS_PATH + "/MaximUsed.wav");
                                int hp = (Core.CharacterData.Hp + 50);
                                if(hp > Core.CharacterData.MaxHP)
                                {
                                    hp = Core.CharacterData.MaxHP;
                                }
                                Core.SetSceneDialog("아이템 사용", "커피 다운 커피 맥심, 현재 체력 : " + hp);
                                Core.CharacterData.UseConsumable(UIManager.selected);
                            }
                            else
                            {
                                Core.PlaySFX(Define.SOUNDS_PATH + "/ItemEquip.wav");
                                Core.CharacterData.Equip(UIManager.selected);
                            }
                        }
                        break;
                    case RightUI.SHOP_PURCHASE:
                        GameManager.Purchase();
                        break;
                    case RightUI.SHOP_SELL:
                        GameManager.Sell();
                        break;
                    case RightUI.BLACKSMITH:
                        break;
                }
            }
            else if(ipt == 'B')
            {
                switch (UIManager.RightUIStatus)
                {
                    case RightUI.CHARA_STATUS:
                        Core.GetEvent("종료하기").Invoke();
                        break;
                    default:
                        UIManager.RightUIStatus = RightUI.CHARA_STATUS;
                        break;
                }
            }
            else if(ipt >= 'a' && ipt <= 'z')
            {
                UIManager.selected = ipt - 'a';
            }
        }
    }
}
