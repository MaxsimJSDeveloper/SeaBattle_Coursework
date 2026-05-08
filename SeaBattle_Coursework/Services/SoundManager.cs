using System.IO;
using System.Windows.Media;

namespace SeaBattle_Coursework.Services
{
    public static class SoundManager
    {
        private static MediaPlayer _musicPlayer = new MediaPlayer();
        private static Dictionary<string, MediaPlayer> _soundEffects = new Dictionary<string, MediaPlayer>();

        private const double MusicVolume = 0.05;
        private const double EffectsVolume = 0.15;

        public static bool IsMusicMuted { get; private set; }
        public static bool IsEffectsMuted { get; private set; }

        static SoundManager()
        {
            _musicPlayer.Volume = MusicVolume;

            _musicPlayer.MediaEnded += (s, e) =>
            {
                _musicPlayer.Stop();
                _musicPlayer.Position = TimeSpan.Zero;

                if (!IsMusicMuted)
                {
                    _musicPlayer.Play();
                }
            };
        }

        private static string GetPath(string fileName)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Sounds", fileName);
        }

        public static void PlayMusic(string fileName)
        {
            _musicPlayer.Open(new Uri(GetPath(fileName)));

            if (!IsMusicMuted)
            {
                _musicPlayer.Play();
            }
        }

        public static void ToggleMusic()
        {
            IsMusicMuted = !IsMusicMuted;
            if (IsMusicMuted) _musicPlayer.Pause();
            else _musicPlayer.Play();
        }

        public static void PlaySound(string fileName)
        {
            if (IsEffectsMuted) return;

            if (!_soundEffects.ContainsKey(fileName))
            {
                var newPlayer = new MediaPlayer();
                newPlayer.Volume = EffectsVolume;
                newPlayer.Open(new Uri(GetPath(fileName)));
                _soundEffects[fileName] = newPlayer;
            }

            var player = _soundEffects[fileName];
            player.Stop();
            player.Position = TimeSpan.Zero;
            player.Play();
        }

        public static void ToggleEffects()
        {
            IsEffectsMuted = !IsEffectsMuted;
        }
        public static void DisposeAll()
        {
            if (_musicPlayer != null)
            {
                _musicPlayer.Stop();
                _musicPlayer.Close();
            }

            foreach (var player in _soundEffects.Values)
            {
                player.Stop();
                player.Close();
            }

            _soundEffects.Clear();
        }
    }
}