using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace SeaBattle_Coursework.Services
{
    public static class SoundManager
    {
        private static MediaPlayer _musicPlayer = new MediaPlayer();
        private static Dictionary<string, MediaPlayer> _soundEffects = new Dictionary<string, MediaPlayer>();

        private const double MusicVolume = 0.05;
        private const double EffectsVolume = 0.2;

        public static bool IsMusicMuted { get; private set; }
        public static bool IsEffectsMuted { get; private set; }

        static SoundManager()
        {
            _musicPlayer.Volume = MusicVolume;

            // ФИКС ЗАЦИКЛИВАНИЯ
            _musicPlayer.MediaEnded += (s, e) =>
            {
                _musicPlayer.Stop(); // Жестко сбрасываем внутренний стейт Windows-плеера
                _musicPlayer.Position = TimeSpan.Zero; // Отматываем

                // Включаем заново, только если игрок не замутил игру
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

        // --- МУЗЫКА ---
        public static void PlayMusic(string fileName)
        {
            // ФИКС МУТА: Мы ВСЕГДА должны загружать новый трек, даже если звук выключен.
            // Иначе при снятии галочки "Мут" на новом экране будет играть старый трек.
            _musicPlayer.Open(new Uri(GetPath(fileName)));

            if (!IsMusicMuted)
            {
                _musicPlayer.Play();
            }
        }

        public static void StopMusic()
        {
            _musicPlayer.Stop();
        }

        public static void ToggleMusic()
        {
            IsMusicMuted = !IsMusicMuted;
            if (IsMusicMuted) _musicPlayer.Pause();
            else _musicPlayer.Play();
        }

        // --- ЭФФЕКТЫ ---
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
    }
}