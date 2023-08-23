namespace SpartaDungeon.Common
{
    public static class Define
    {
        public const int SCENE_MENU_KEY       = 0;
        public const int SCENE_TOWN_KEY       = 1;
        public const int SCENE_DUNGEON_KEY    = 2;
        public const int SCENE_INN_KEY        = 3;
        public const int SCENE_SHOP_KEY       = 4;
        public const int SCENE_BLACKSMITH_KEY = 5;

        public static readonly string LOCAL_GAME_PATH   = System.IO.Directory.GetParent(System.Environment.CurrentDirectory).Parent.Parent.FullName.ToString();
        public static readonly string GAME_SAVE_PATH    = LOCAL_GAME_PATH + "/Data/CharacterData.json";
        public static readonly string ITEMBASE_PATH     = LOCAL_GAME_PATH + "/Data/ItemBase.csv";
        public static readonly string SCENES_PATH       = LOCAL_GAME_PATH + "/Scenes";
        public static readonly string RESOURCES_PATH    = LOCAL_GAME_PATH + "/Resources";
        public static readonly string SOUNDS_PATH       = LOCAL_GAME_PATH + "/Sounds";

        public const int SFX_POOL_SIZE        = 10;

        public const int SCREEN_WIDTH         = 98;
        public const int SCREEN_HEIGHT        = 38;

        public const int UI_RIGHT_PIVOT       = 62;
        public const int UI_RIGHT_WIDTH       = 35;
        public const int UI_RIGHT_HEIGHT      = 31;

        public const int UI_RIGHT_CMD_PIVOT_X = 65;
        public const int UI_RIGHT_CMD_PIVOT_Y = 28;

        public const int UI_RIGHT_INVENTORY_DIALOG_PIVOT_X = 64;
        public const int UI_RIGHT_INVENTORY_DIALOG_PIVOT_Y = 22;

        public const int UI_BOTTOM_PIVOT      = 31;
        public const int UI_BOTTOM_WIDTH      = 97;
        public const int UI_BOTTOM_HEIGHT     = 5;
                                              
        public const int UI_CMD_PIVOT         = 36;
    }
}
