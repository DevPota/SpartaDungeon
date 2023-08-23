using System.Media;

namespace SpartaDungeon.Classes
{
    class AudioSource
    {
        SoundPlayer player { get; set; }

        public void Play()
        {
            ThreadPool.QueueUserWorkItem(_ => player.Play());
        }

        public void SetClip(string path)
        {
            player = new SoundPlayer(new FileStream(path, FileMode.Open, FileAccess.Read));
        }
    }
}
