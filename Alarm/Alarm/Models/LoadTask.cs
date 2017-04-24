using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Alarm.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Alarm.Models
{
    public class LoadTask
    {
        private static string path = @"Datas/Task.json";
        private static string fontSizePath = "Datas/fontSize.txt";
        private static readonly string autidoPath;
        public static List<TaskItem> AllTask;
        public static List<double> FontSize;
        public static string BinDir;

        static LoadTask()
        {
            try
            {
                BinDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                path = Path.Combine(BinDir, @"Datas/Task.json");
                fontSizePath = Path.Combine(BinDir, "Datas/fontSize.txt");
                // 设置音乐文件路径
                autidoPath = Path.Combine(Directory.GetCurrentDirectory(), "Audio");
                FontSize = GetFontSizes();
                Reload();
            }
            catch (Exception e)
            {
                Logger.Log("LoadTask" + e.Message);
            }
        }

        public static List<double> GetFontSizes()
        {
            var list = new List<double>();
            var sizes = File.ReadAllLines(fontSizePath);
            for (var i = 0; i < sizes.Length; i++)
            {
                double size;
                if (double.TryParse(sizes[i], out size))
                    list.Add(size);
            }
            // 默认字号
            if (list.Count() == 0)
                list.Add(24);
            return list;
        }

        public static bool Save()
        {
            try
            {
                AllTask = AllTask.Where(x => x.State != TaskState.Invaild).ToList();
                var str = JsonConvert.SerializeObject(AllTask);
                if (File.Exists(path))
                    File.WriteAllText(path, str);
                return true;
            }
            catch (Exception exp)
            {
                return false;
            }
        }

        public static void Reload()
        {
            AllTask = GetAllTask();
        }

        #region Load Audio

        public static List<string> LoadAudio()
        {
            if (!Directory.Exists(autidoPath))
                return new List<string>();
            var files = GetFiles(autidoPath, "*.wmv|*.mp3|*.wma|*.wav", SearchOption.TopDirectoryOnly);
            return files.Select(x => Path.GetFileName(x)).ToList();
        }

        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            var searchPatterns = searchPattern.Split('|');
            var files = new List<string>();
            foreach (var sp in searchPatterns)
                files.AddRange(Directory.GetFiles(path, sp, searchOption));
            files.Sort();
            return files.ToArray();
        }

        public static string GetPathByName(string soundName)
        {
            return Path.Combine(autidoPath, soundName);
        }

        #endregion

        #region 增删改查

        public static bool DelTask(string Id)
        {
            var task = AllTask.Where(x => x.Id == Id).ToList();
            for (var i = 0; i < task.Count(); i++)
                AllTask.Remove(task[i]);
            Save();
            return true;
        }

        public static bool AddTask(TaskItem task)
        {
            var item = FindTask(task.Id);
            if (item == null)
            {
                AllTask.Add(task);
            }
            return Save();
        }

        public static TaskItem FindTask(string Id)
        {
            if (AllTask == null) return null;
            var item = AllTask.FirstOrDefault(x => x.Id == Id);
            return item;
        }

        public static List<TaskItem> GetAllTask()
        {
            var tasks = new List<TaskItem>();
            try
            {
                if (File.Exists(path))
                {
                    var task = File.ReadAllText(path);
                    var token = JArray.Parse(task);
                    tasks = token.ToObject<List<TaskItem>>();
                    return tasks;
                }
                else
                {
                    Logger.Log("tasklist " + path);
                    return tasks;
                }
            }
            catch (Exception ex)
            {
                Logger.Log("GetAllTask" + ex.Message);
                return tasks;
            }
        }

        #endregion
    }
}