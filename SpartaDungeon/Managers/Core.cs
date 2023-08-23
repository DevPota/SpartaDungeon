using System.Text;
using System.Text.Json;
using SpartaDungeon.Common;
using SpartaDungeon.Classes;

namespace SpartaDungeon.Managers
{
    class SceneModule
    {
        public Scene[]                    BuildScenes        { get; set; }   = null;
        public Scene                      CurrentScene       { get; set; }   = null;
        public Dictionary<string, Action> EventBase                          = new Dictionary<string, Action>();
        public string                     DialogActorName    { get; set; }
        public string                     Dialog             { get; set; }
    }

    class SoundModule
    {
        Queue<AudioSource> sfxQueue = new Queue<AudioSource>();

        public void Init()
        {
            for(int i = 0; i < 10; i++)
            {
                sfxQueue.Enqueue(new AudioSource());
            }
        }

        public void PlaySFX(string path)
        {
            AudioSource temp = sfxQueue.Dequeue();
            temp.SetClip(path);
            temp.Play();
            sfxQueue.Enqueue(temp);
        }
    }

    class InputModule
    {
        public string[] Pairs { get; set; } = null;

        public char GetValidInputCmd()
        {
            char ipt;
            bool isValid = char.TryParse(Console.ReadLine(), out ipt);

            if (isValid == true)
            {
                return ipt;
            }
            else
            {
                return '0';
            }
        }
    }

    class DataModule
    {
        public void SaveData<T>(string path, T jsonClass) where T : class
        {
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(jsonClass);
            File.WriteAllText(path, Encoding.UTF8.GetString(bytes));
        }

        public string LoadData(string path)
        {
            if (File.Exists(path) == true)
            {
                return File.ReadAllText(path);
            }
            else
            {
                return null;
            }
        }

        public T LoadData<T>(string path) where T : new()
        {
            if (File.Exists(path) == true)
            {
                return JsonSerializer.Deserialize<T>(File.ReadAllText(path));
            }
            else
            {
                return new T();
            }
        }
    }

    class ResourceModule
    {
        Dictionary<string, string> ResourceBase = new Dictionary<string, string>();

        public void LoadResource(string name, string element)
        {
            ResourceBase.Add(name, element);
        }

        public string GetResource(string name)
        {
            return ResourceBase.GetValueOrDefault(name);
        }
    }

    public static class Core
    {
        public static GameCharacter CharacterData { get; private set; } = null;

        static SceneModule    Scene    { get; set; } = new SceneModule();
        static SoundModule    Sound    { get; set; } = new SoundModule();
        static InputModule    Input    { get; set; } = new InputModule();
        static DataModule     Data     { get; set; } = new DataModule();
        static ResourceModule Resource { get; set; } = new ResourceModule();

        public static void Init()
        {
            // Data Init
            CharacterData = Data.LoadData<GameCharacter>(Define.GAME_SAVE_PATH);
            Data.SaveData(Define.GAME_SAVE_PATH, CharacterData);

            // Resource Init
            string[] resourceFilePathes = Directory.GetFiles(Define.RESOURCES_PATH);

            for (int i = 0; i < resourceFilePathes.Length; i++)
            {
                Resource.LoadResource(Path.GetFileNameWithoutExtension(
                    resourceFilePathes[i]),

                    File.ReadAllText(Define.RESOURCES_PATH +
                    "/" +
                    Path.GetFileName(resourceFilePathes[i])));
            }

            string   itemBaseText = Data.LoadData(Define.ITEMBASE_PATH);
            string[] lines        = itemBaseText.Split('\n');

            foreach(string itemData in lines)
            {
                string[] elements = itemData.Split(',');

                Resource.LoadResource(elements[0], itemData);
            }

            // Sound Init
            Sound.Init();

            // Scene Init
            string[] sceneFilePathes = Directory.GetFiles(Define.SCENES_PATH);
            Scene.BuildScenes        = new Scene[sceneFilePathes.Length];

            for (int i = 0; i < Scene.BuildScenes.Length; i++)
            {
                Scene.BuildScenes[i] = Data.LoadData<Scene>
                    (Define.SCENES_PATH +
                    "/" +
                    Path.GetFileName(sceneFilePathes[i]
                    ));
            }

            Scene.EventBase.Add("시작하기",                () => { GetEvent(GetScene(CharacterData.Location).Name)(); });
            Scene.EventBase.Add("종료하기",                () => { Program.applicationQuit = true; });
            Scene.EventBase.Add("EV_Menu",                () => { LoadScene(Define.SCENE_MENU_KEY); SetSceneDialog("", "환영합니다. 커맨드를 입력하여 진행 할 수 있습니다"); PlaySFX(Define.SOUNDS_PATH + "/Menu.wav"); });
            Scene.EventBase.Add("내일배움캠프",            () => { LoadScene(Define.SCENE_TOWN_KEY); SetSceneDialog("", "내일배움캠프에 오신 것을 환영 합니다!"); });
            Scene.EventBase.Add("공부하기",                () => { LoadScene(Define.SCENE_DUNGEON_KEY); SetSceneDialog("박종민 매니저", "안녕하세요?"); PlaySFX(Define.SOUNDS_PATH + "/DungeonEnter.wav"); });
            Scene.EventBase.Add("도망가기",                () => { LoadScene(Define.SCENE_TOWN_KEY); SetSceneDialog("박종민 매니저", "어디가세요?"); PlaySFX(Define.SOUNDS_PATH + "/DungeonExit.wav"); });
            Scene.EventBase.Add("EV_LevelUp",             () => { SetSceneDialog("박종민 매니저", "좀 더 강해지셨군요 (레벨업)"); PlaySFX(Define.SOUNDS_PATH + "/LevelUp.wav"); });
            Scene.EventBase.Add("한별 쉼터",               () => { LoadScene(Define.SCENE_INN_KEY); SetSceneDialog("이한별 튜터", "잘왔다"); PlaySFX(Define.SOUNDS_PATH + "/InnEnter.wav"); });
            Scene.EventBase.Add("EV_GameOver",            () => { LoadScene(Define.SCENE_INN_KEY); SetSceneDialog("이한별 튜터", "쉬엄쉬엄해라 (체력이 0이 되어 쓰러짐, 보유 코인 50% 잃음)"); PlaySFX(Define.SOUNDS_PATH + "/GameOver.wav"); });
            Scene.EventBase.Add("쉼터 나가기",             () => { LoadScene(Define.SCENE_TOWN_KEY); SetSceneDialog("이한별 튜터", "잘가라"); PlaySFX(Define.SOUNDS_PATH + "/InnExit.wav"); });
            Scene.EventBase.Add("쉬기",                   () => { GameManager.Rest();});
            Scene.EventBase.Add("김영호 하드웨어 샵",        () => { LoadScene(Define.SCENE_SHOP_KEY); SetSceneDialog("김영호 튜터", "어서오시게"); PlaySFX(Define.SOUNDS_PATH + "/ShopEnter.wav"); });
            Scene.EventBase.Add("구매하기",                 () => { UIManager.selected = 0; UIManager.RightUIStatus = RightUI.SHOP_PURCHASE; });
            Scene.EventBase.Add("판매하기",                 () => { UIManager.selected = 0; UIManager.RightUIStatus = RightUI.SHOP_SELL; });
            Scene.EventBase.Add("상점 나가기",              () => { 
                                                                  if(UIManager.RightUIStatus == RightUI.SHOP_PURCHASE || UIManager.RightUIStatus == RightUI.SHOP_SELL)
                                                                  {
                                                                        UIManager.RightUIStatus = RightUI.CHARA_STATUS;
                                                                  }
                                                                  LoadScene(Define.SCENE_TOWN_KEY); 
                                                                  SetSceneDialog("김영호 튜터", "잘가게"); 
                                                                  PlaySFX(Define.SOUNDS_PATH + "/ShopExit.wav"); });
            Scene.EventBase.Add("강인의 튜닝 스테이션",             () => { LoadScene(Define.SCENE_BLACKSMITH_KEY); SetSceneDialog("강인 튜터", "반갑군"); PlaySFX(Define.SOUNDS_PATH + "/BlacksmithEnter.wav"); });
            Scene.EventBase.Add("장착 키보드 강화",                 () => { GameManager.Enchant(ItemType.WEAPON, CharacterData.Weapon); });
            Scene.EventBase.Add("장착 마우스 강화",                 () => { GameManager.Enchant(ItemType.ARMOR, CharacterData.Armor); });
            Scene.EventBase.Add("장착 수리",                       () => { GameManager.Repair(); });
            Scene.EventBase.Add("튜닝 스테이션 나가기",             () => { LoadScene(Define.SCENE_TOWN_KEY); SetSceneDialog("강인 튜터", "또 와라"); PlaySFX(Define.SOUNDS_PATH + "/BlacksmithExit.wav"); });
            Scene.EventBase.Add("자습하기(근성10)",                () => { GameManager.PlayDungeon(0); });
            Scene.EventBase.Add("개인 숙제하기(근성20)",            () => { GameManager.PlayDungeon(1); });
            Scene.EventBase.Add("팀 프로젝트 하기(근성40)",         () => { GameManager.PlayDungeon(2); });
        }

        public static Scene GetScene(int sceneIndex)
        {
            return Scene.BuildScenes[sceneIndex];
        }

        public static void LoadScene(int sceneIndex)
        {
            Scene.CurrentScene = Scene.BuildScenes[sceneIndex];
            Input.Pairs = Scene.CurrentScene.SceneActionKeys;
            if (sceneIndex > 0)
            {
                CharacterData.Location = sceneIndex;
                SaveData();
            }
        }

        public static void SaveData()
        {
            Data.SaveData(Define.GAME_SAVE_PATH, CharacterData);
        }

        public static void Render()
        {
            if (Scene.CurrentScene == null)
            {
                return;
            }
            else
            {
                Console.WriteLine(Resource.GetResource(Scene.CurrentScene.SpriteKey));
            }
        }

        public static Scene GetCurrentScene()
        {
            return Scene.CurrentScene;
        }

        public static char GetInput()
        {
            return Input.GetValidInputCmd();
        }

        public static Action GetEvent(int cmd)
        {
            return Scene.EventBase.GetValueOrDefault(Input.Pairs[cmd]);
        }

        public static Action GetEvent(string cmdName)
        {
            return Scene.EventBase.GetValueOrDefault(cmdName);
        }

        public static string GetResource(string resourceKey)
        {
            return Resource.GetResource(resourceKey);
        }

        public static void SetSceneDialog(string name, string text)
        {
            Scene.DialogActorName = name;
            Scene.Dialog = text;
        }

        public static string[] GetDialogData()
        {
            return new string[2] { Scene.DialogActorName, Scene.Dialog };
        }

        public static int GetCmdRange()
        {
            return Scene.CurrentScene.SceneActionKeys.Length;
        }

        public static void PlaySFX(string path)
        {
            Sound.PlaySFX(path);
        }
    }
}
