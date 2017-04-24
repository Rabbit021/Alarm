using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Alarm.Models;

namespace Alarm.Common
{
    public class Helper
    {
        private static MediaPlayer _player = new MediaPlayer();

        static Helper()
        {
            _player.MediaEnded -= _player_MediaEnded;
            _player.MediaEnded += _player_MediaEnded;
        }

        static void _player_MediaEnded(object sender, EventArgs e)
        {
            _player.Position = TimeSpan.Zero;
         //   _player.Play();
        }

        /// <summary>
        ///     停止播放声音
        /// </summary>
        public static void StopAudio()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    if (_player != null)
                        _player.Stop();
                    _player = null;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.Message);
                }
            }));
        }

        /// <summary>
        ///     播放声音
        /// </summary>
        /// <param name="soundName">声音名</param>
        public static void PalyAudio(string soundName, double volume)
        {
            if (string.IsNullOrEmpty(soundName) || soundName.Contains("无"))
                return;

            var path = LoadTask.GetPathByName(soundName);
            if (!File.Exists(path))
                return;
            if (_player != null)
            {
                StopAudio();
            }
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    _player = new MediaPlayer();
                    _player.Open(new Uri(path));
                    _player.Volume = volume;
                    _player.Play();
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.Message);
                }
            }));
        }

        public static void AdjustVolume(string soundName, double volume)
        {
            try
            {
                if (_player != null)
                {
                    _player.Volume = volume/100;
                }
                else
                {
                    PalyAudio(soundName, volume);
                }
            }
            catch (Exception exp)
            {
                Logger.Log(exp.Message);
            }
        }
    }
}