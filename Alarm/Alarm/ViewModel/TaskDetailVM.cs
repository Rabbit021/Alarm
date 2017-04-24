using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Alarm.Common;
using Alarm.Models;
using Alarm.View;
using Microsoft.Expression.Interactivity.Core;

namespace Alarm.ViewModel
{
    public class TaskDetailVM : BaseVM
    {
        private TaskDetailVM()
        {
            PageName = "TaskDetail";
        }

        public bool IsSave { get; set; }

        private ShowInfo _PreView;
        public ShowInfo PreView
        {
            get
            {
                if (_PreView == null)
                    return new ShowInfo();
                return _PreView;
            }
            set
            {
                _PreView = value;
            }
        }

        public ICommand PreviewCommand { get; set; }

        public override void Loaded()
        {
            base.Loaded();
            InitData();
        }

        private void InitData()
        {
            try
            {
                var typeCollection = new ObservableCollection<TaskType>();
                foreach (var itr in Enum.GetValues(typeof(TaskType)))
                    typeCollection.Add((TaskType)itr);
                TypeCollection = typeCollection;
                if (CurrentTask == null)
                {
                    var item = new TaskItem();
                    item.GetDefaultTask();
                    CurrentTask = item;
                }
                FontSizes = new ObservableCollection<double>(LoadTask.FontSize);
                AllAudio = new ObservableCollection<string>(LoadTask.LoadAudio());
                PreviewCommand = new ActionCommand(TaskPreView);
            }
            catch (Exception ex)
            {
                Logger.Log("InitData"+ex.Message);
            }
        }

        public void Save()
        {
            IsSave = true;
            LoadTask.AddTask(CurrentTask);
            TaskListVM.Instance.Refresh();
        }

        public bool Cancle()
        {
            IsSave = false;
            LoadTask.Reload();
            return true;
        }

        private void TaskPreView()
        {
            PreView.IsPreview = true;
            PreView.ShowPreView(CurrentTask);
        }

        #region Instance

        private static readonly TaskDetailVM instance = new TaskDetailVM();

        public static TaskDetailVM Instance
        {
            get { return instance; }
        }

        #endregion

        #region  CurrentTask

        public TaskItem CurrentTask
        {
            get { return (TaskItem)GetValue(CurrentTaskProperty); }
            set { SetValue(CurrentTaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentTaskProperty =
            DependencyProperty.Register("CurrentTask", typeof(TaskItem), typeof(TaskDetailVM),
                new PropertyMetadata((sender, e) =>
                {
                    var vm = sender as TaskDetailVM;
                    if (vm == null) return;
                }));

        #endregion

        #region TypeCollection

        public ObservableCollection<TaskType> TypeCollection
        {
            get { return (ObservableCollection<TaskType>)GetValue(TypeCollectionProperty); }
            set { SetValue(TypeCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TypeCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeCollectionProperty =
            DependencyProperty.Register("TypeCollection", typeof(ObservableCollection<TaskType>), typeof(TaskDetailVM),
                new PropertyMetadata(
                    (sender, e) => { }));

        #endregion

        #region AllAudio

        public ObservableCollection<string> AllAudio
        {
            get { return (ObservableCollection<string>)GetValue(AllAudioProperty); }
            set { SetValue(AllAudioProperty, value); }
        }

        public static readonly DependencyProperty AllAudioProperty =
            DependencyProperty.Register("AllAudio", typeof(ObservableCollection<string>), typeof(TaskDetailVM),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var vm = sender as TaskDetailVM;
                        if (vm == null) return;
                    }));

        #endregion

        #region FontSizes

        public ObservableCollection<double> FontSizes
        {
            get { return (ObservableCollection<double>)GetValue(FontSizesProperty); }
            set { SetValue(FontSizesProperty, value); }
        }

        public static readonly DependencyProperty FontSizesProperty =
            DependencyProperty.Register("FontSizes", typeof(ObservableCollection<double>), typeof(TaskListVM),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var vm = sender as TaskListVM;
                        if (vm == null) return;
                    }));

        #endregion
    }
}