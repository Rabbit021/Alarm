using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Alarm.Models;
using Alarm.ViewModel;

namespace Alarm.View
{
    /// <summary>
    ///     TaskList.xaml 的交互逻辑
    /// </summary>
    public partial class TaskList : UserControl
    {
        public TaskList()
        {
            InitializeComponent();
            this.Loaded += TaskList_Loaded;
        }

        void TaskList_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void edtBtn_Click(object sender, RoutedEventArgs e)
        {
            TaskListVM.Instance.EditTask(null);
        }

        private void delBtn_Click(object sender, RoutedEventArgs e)
        {
            TaskListVM.Instance.DelTask(null);
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            if (TaskListVM.Instance.SelectedTask != null)
                TaskListVM.Instance.EditTask(null);
        }

    }
}