using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Newtonsoft.Json;

namespace Alarm.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TaskItem : DependencyObject
    {
        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public TaskState State { get; set; }

        public void GetDefaultTask()
        {
            var week = new ObservableCollection<WeekNode>();

            week.Add(new WeekNode {Name = "星期一", Week = DayOfWeek.Monday});
            week.Add(new WeekNode {Name = "星期二", Week = DayOfWeek.Tuesday});
            week.Add(new WeekNode {Name = "星期三", Week = DayOfWeek.Wednesday});
            week.Add(new WeekNode {Name = "星期四", Week = DayOfWeek.Thursday});
            week.Add(new WeekNode {Name = "星期五", Week = DayOfWeek.Friday});
            week.Add(new WeekNode {Name = "星期六", Week = DayOfWeek.Saturday});
            week.Add(new WeekNode {Name = "星期日", Week = DayOfWeek.Sunday});

            Id = Guid.NewGuid().ToString();
            Title = string.Empty;
            Content = string.Empty;
            ItemType = TaskType.Once;
            State = TaskState.Normal;
            WeekDays = week;
            Audio = string.Empty;
            Volume = 10;
            AlarmFontSize = 30;
            Date = DateTime.Now.ToShortDateString();
            Time = DateTime.Now.ToLongTimeString();
        }

        public TaskItem()
        {
            
        }

        #region Title

        [JsonProperty]
        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof (string), typeof (TaskItem),
                new PropertyMetadata((sender, e) => { }));

        #endregion

        #region Content

        [JsonProperty]
        public string Content
        {
            get { return (string) GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof (string), typeof (TaskItem), new PropertyMetadata(
                (sender, e) => { }));

        #endregion

        #region TaskType

        [JsonProperty]
        public TaskType ItemType
        {
            get { return (TaskType) GetValue(ItemTypeProperty); }
            set { SetValue(ItemTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TaskType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTypeProperty =
            DependencyProperty.Register("ItemType", typeof (TaskType), typeof (TaskItem),
                new PropertyMetadata((sender, e) => { }));

        #endregion

        #region Time

        [JsonProperty]
        public string Time
        {
            get { return (string) GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof (string), typeof (TaskItem),
                new PropertyMetadata((sender, e) => { }));

        #endregion

        #region Date

        [JsonProperty]
        public string Date
        {
            get { return (string) GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Date.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof (string), typeof (TaskItem),
                new PropertyMetadata((sender, e) => { }
                    ));

        #endregion

        #region WeekDays

        [JsonProperty]
        public ObservableCollection<WeekNode> WeekDays
        {
            get { return (ObservableCollection<WeekNode>) GetValue(WeekDaysProperty); }
            set { SetValue(WeekDaysProperty, value); }
        }

        public static readonly DependencyProperty WeekDaysProperty =
            DependencyProperty.Register("WeekDays", typeof (ObservableCollection<WeekNode>), typeof (TaskItem),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var vm = sender as TaskItem;
                        if (vm == null) return;
                    }));

        #endregion

        #region Audio

        [JsonProperty]
        public string Audio
        {
            get { return (string) GetValue(AudioProperty); }
            set { SetValue(AudioProperty, value); }
        }

        public static readonly DependencyProperty AudioProperty =
            DependencyProperty.Register("Audio", typeof (string), typeof (TaskItem), new PropertyMetadata(
                (sender, e) =>
                {
                    var vm = sender as TaskItem;
                    if (vm == null) return;
                }));

        #endregion

        #region Volume

        [JsonProperty]
        public double Volume
        {
            get { return (double) GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof (double), typeof (TaskItem), new PropertyMetadata(
                (sender, e) =>
                {
                    var vm = sender as TaskItem;
                    if (vm == null) return;
                }));

        #endregion

        #region AlarmFontSize

        [JsonProperty]
        public double AlarmFontSize
        {
            get { return (double)GetValue(AlarmFontSizeProperty); }
            set { SetValue(AlarmFontSizeProperty, value); }
        }

        public static readonly DependencyProperty AlarmFontSizeProperty =
            DependencyProperty.Register("AlarmFontSize", typeof(double), typeof(TaskItem), new PropertyMetadata(
                (sender, e) =>
                {
                    var vm = sender as TaskItem;
                    if (vm == null) return;
                }));

        #endregion
    }

    public class TaskItemPrase
    {
        public string Id { get; set; }
        public TaskState State { get; set; }
        public string Title { get; set; }

        public string Content { get; set; }
        public TaskType ItemType { get; set; }
        public string Time { get; set; }

        public string Date { get; set; }
        public List<WeekNode> WeekDays { get; set; }
        public string Audio { get; set; }

        public double Volume { get; set; }
        public double AlarmFontSize { get; set; }

    }
    public enum TaskType
    {
        Once = 0,
        EveryDay = 1,
        EveryWeek = 2,
        EveryMonth = 3
    }

    public enum TaskState
    {
        Normal = 0,
        Alarmed = 1,
        Invaild = 2,
        WaitDeal = 3
    }
}