using System.Text.Json;

namespace SpartaDungeon.Classes
{
    public class Scene
    {
        public string   Name { get; set; }
        public string   SpriteKey { get; set; }
        public string[] SceneActionKeys { get; set; }
    }
}
