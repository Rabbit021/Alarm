using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Alarm.Common;
using Alarm.Models;
using Alarm.View;
using Microsoft.Expression.Interactivity.Core;

namespace Alarm.ViewModel
{
    public class TaskListVM : BaseVM
    {
        private TaskListVM()
        {
            PageName = "TaskList";
        }
        private TaskDetail detail;
        public ICommand AddTaskCommand { get; set; }
        public ICommand EditTaskCommand { get; set; }
        public ICommand DelTaskCommand { get; set; }

        public override void Loaded()
        {
            base.Loaded();
            Refresh();
        }

        public override void RegisterCommands()
        {
            AddTaskCommand = new ActionCommand(AddTask);
            EditTaskCommand = new ActionCommand(EditTask);
            DelTaskCommand = new ActionCommand(DelTask);
        }

        public void DelTask(object obj)
        {
            try
            {
                TaskDetailVM.Instance.CurrentTask = null;
                if (SelectedTask == null)
                    return;
                LoadTask.DelTask(SelectedTask.Id);
                Refresh();
                if (Tasks != null && Tasks.Count > 0)
                    SelectedTask = Tasks.Last();
            }
            catch (Exception exp)
            {
                Logger.Log("DelTask"+exp.Message);
            }
        }

        public void EditTask(object obj)
        {
            try
            {
                if (SelectedTask == null)
                    return;
                GetDetailWindow();
                TaskDetailVM.Instance.CurrentTask = LoadTask.FindTask(SelectedTask.Id);
                detail.ShowWindow();
                Refresh();
            }
            catch (Exception exp)
            {
                Logger.Log("EditTask" +exp.Message);
            }
        }

        public void AddTask(object obj)
        {
            try
            {
                GetDetailWindow();
                TaskDetailVM.Instance.CurrentTask = null;
                detail.ShowWindow();
                Refresh();
            }
            catch (Exception exp)
            {
                Logger.Log("AddTask"+exp.Message);
            }
        }

        private void GetDetailWindow()
        {
            //            if (detail == null)
            //                detail = new TaskDetail();
            //            detail.Topmost = true;

            detail = new TaskDetail();

        }
        public void Refresh()
        {
            LoadTask.Reload();
            Tasks = new ObservableCollection<TaskItem>(LoadTask.AllTask);
        }

        #region Tasks

        // Using a DependencyProperty as the backing store for Tasks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TasksProperty =
            DependencyProperty.Register("Tasks", typeof(ObservableCollection<TaskItem>), typeof(TaskListVM),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var vm = sender as TaskListVM;
                        if (vm == null) return;
                    }));

        public ObservableCollection<TaskItem> Tasks
        {
            get { return (ObservableCollection<TaskItem>)GetValue(TasksProperty); }
            set { SetValue(TasksProperty, value); }
        }

        #endregion

        #region selectedTask

        public TaskItem SelectedTask
        {
            get { return (TaskItem)GetValue(SelectedTaskProperty); }
            set { SetValue(SelectedTaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedTasl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTaskProperty =
            DependencyProperty.Register("SelectedTask", typeof(TaskItem), typeof(TaskListVM), new PropertyMetadata(
                (sender, e) =>
                {
                    var vm = sender as TaskListVM;
                    if (vm == null) return;
                }));

        #endregion

        #region Instance

        private static readonly TaskListVM instance = new TaskListVM();

        public static TaskListVM Instance
        {
            get { return instance; }
        }

        #endregion
    }
}