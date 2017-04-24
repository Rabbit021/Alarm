using System;
using System.IO;
using Alarm.Models;

namespace Alarm.Common
{
    public class Logger
    {
        public static void Log(string msg)
        {
            var str = string.Format(@"**--- {0}: {1} \n ", DateTime.Now, msg);
            File.AppendAllText(Path.Combine(LoadTask.BinDir, "log.txt"), str);
        }
    }
}