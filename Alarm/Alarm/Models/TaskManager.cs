namespace Alarm.Models
{
    public class TaskManager
    {

        #region Instance

        private static readonly TaskManager _instance = new TaskManager();

        public static TaskManager Instance
        {
            get { return _instance; }
        }

        #endregion

        private TaskManager()
        {
        }

        public void DealTask(TaskItem itr)
        {
            switch (itr.ItemType)
            {
                case TaskType.Once:
                    itr.State = TaskState.Invaild;
                    break;
                case TaskType.EveryDay:
                    itr.State = TaskState.Normal;
                    break;
                case TaskType.EveryMonth:
                    itr.State = TaskState.Normal;
                    break;
                case TaskType.EveryWeek:
                    itr.State = TaskState.Normal;
                    break;
            }
        }
    }
}