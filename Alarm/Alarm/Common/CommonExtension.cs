using Alarm.Models;

namespace Alarm.Common
{
    public static  class CommoonExtension
    {
        public static string TaskTypeToString(this TaskType taskType)
        {
            var res = string.Empty;
            switch (taskType)
            {
                case TaskType.Once:
                    res = "一次";
                    break;
                case TaskType.EveryDay:
                    res = "每天";
                    break;
                case TaskType.EveryWeek:
                    res = "每周";
                    break;
                case TaskType.EveryMonth:
                    res = "每月";
                    break;
//                case TaskType.EveryYear:
//                    res = "每年";
//                    break;
            }
            return res;
        }

        public static TaskType StringToTaskType(this string taskType)
        {
            var res = TaskType.Once;
            switch (taskType)
            {
                case "一次":
                    res = TaskType.Once;
                    break;
                case "每天":
                    res = TaskType.EveryDay;
                    break;
                case "每周":
                    res = TaskType.EveryWeek;
                    break;
                case "每月":
                    res = TaskType.EveryMonth;
                    break;
//                case "每年":
//                    res = TaskType.EveryYear;
//                    break;
            }
            return res;
        } 
    }
}