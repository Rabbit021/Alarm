using System;
using System.IO;
using Alarm.Common;
using Newtonsoft.Json;

namespace Alarm.Models
{
    public class Config
    {
        public static ConfigItem Current;

        static Config()
        {
            try
            {
                var str = File.ReadAllText("Config.json");
                Current = JsonConvert.DeserializeObject<ConfigItem>(str);
            }
            catch (Exception exp)
            {
                Logger.Log("Config"+exp.Message);
            }
        }
    }

    public class ConfigItem
    {
        public int AlarmAudioSpan { get; set; }
        public int CloseAlarmSpan { get; set; }
        public bool AlarmTopMost { get; set; }
        public string HeaderColor { get; set; }
        public bool HeaderTwoPart { get; set; }
        public bool CanShake { get; set; }
        public bool CanSplash { get; set; }
        public string SplashColor { get; set; }
        public string Password { get; set; }
    }
}