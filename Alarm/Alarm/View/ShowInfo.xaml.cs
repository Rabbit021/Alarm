using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Alarm.Common;
using Alarm.Models;
using Alarm.ViewModel;

namespace Alarm.View
{
    /// <summary>
    ///     ShowInfo.xaml 的交互逻辑
    /// </summary>
    public partial class ShowInfo : Window
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();
        public Storyboard board;
        private int count = 3;
        public int CurrentPos;
        public bool IsClosed;
        public bool IsPreview;
        public DateTime PlaySoundTime;
        public Storyboard SplashBoard;

        public ShowInfo()
        {
            InitializeComponent();
            Topmost = Config.Current.AlarmTopMost;
            if (!Config.Current.HeaderTwoPart)
                headerBg.Background = null;
            Left = 0.0;
            Top = 0.0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;

            board = Resources["Shake"] as Storyboard;
            SplashBoard = Resources["Splash"] as Storyboard;
            Loaded += ShowInfo_Loaded;
            Closing += ShowInfo_Closing;
            PreviewKeyDown += ShowInfo_PreviewKeyDown;
            WindowStartupLocation = WindowStartupLocation.Manual;
        }

        private void ShowInfo_Closing(object sender, CancelEventArgs e)
        {
            Hide();
        }

        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            var span = DateTime.Now - PlaySoundTime;
            if (span.TotalSeconds > Config.Current.AlarmAudioSpan && PlaySoundTime != DateTime.MinValue)
                player.Stop();
            else
            {
                player.Position = TimeSpan.Zero;
            }
        }

        private void ShowInfo_Loaded(object sender, RoutedEventArgs e)
        {
            var color = (Color)ColorConverter.ConvertFromString(Config.Current.HeaderColor);
            header.Foreground = new SolidColorBrush(color);
            DataContext = this;
            if (AlarmedTaskList == null)
                return;
            foreach (var itr in AlarmedTaskList)
                itr.State = TaskState.WaitDeal;
            CurrentPos = 0;
            GetItemAtPosition();
        }

        private void ShowInfo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
            IsClosed = true;
            if (AlarmedTaskList == null) return;
            foreach (var itr in AlarmedTaskList)
                TaskManager.Instance.DealTask(itr);
            LoadTask.Save();
            player.Stop();
            TaskListVM.Instance.Refresh();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            CurrentPos++;
            GetItemAtPosition();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            CurrentPos--;
            GetItemAtPosition();
        }

        public void BuidCollectionView()
        {
            if (AlarmedTaskList.Count != 0)
                ShowWindow();
            else
                CloseWindow();
            GetItemAtPosition();
        }

        private void GetItemAtPosition()
        {
            if (AlarmedTaskList == null)
                return;
            CurrentPos = Math.Min(CurrentPos, AlarmedTaskList.Count - 1);
            CurrentPos = Math.Max(CurrentPos, 0);
            try
            {
                SelectedItem = AlarmedTaskList[CurrentPos];
            }
            catch (Exception exp)
            {
                // SelectedItem = null;
            }
            var span = DateTime.Now - PlaySoundTime;
            if (span.TotalSeconds > Config.Current.AlarmAudioSpan && PlaySoundTime != DateTime.MinValue)
                player.Stop();
            var count = AlarmedTaskList.Count;
            CountItem = string.Format("{0}/{1}", CurrentPos + 1, count);
            Next.IsEnabled = count > 1 && CurrentPos < count - 1;
            Previous.IsEnabled = count > 1 && CurrentPos > 0;
        }

        private void PlaySound()
        {
            if (SelectedItem == null)
            {
                player.Stop();
                return;
            }
            player.Source = new Uri(LoadTask.GetPathByName(SelectedItem.Audio));
            player.Volume = SelectedItem.Volume;
            PlaySoundTime = DateTime.Now;
            player.Play();
        }

        public void CheckAlarm(List<TaskItem> tasks)
        {
            var alarmTask = tasks.Where(x => x.State == TaskState.WaitDeal || x.State == TaskState.Alarmed).ToList();
            var res = alarmTask.OrderByDescending(x => DateTime.Parse(x.Time));
            var Ids = new List<string>();
            Ids.AddRange(res.Select(x => x.Id));
            Ids.AddRange(AlarmedTaskList.Select(x => x.Id));
            if (Ids.Distinct().Count() != AlarmedTaskList.Count())
                AlarmedTaskList = new ObservableCollection<TaskItem>(res);
        }

        public void ShowPreView(TaskItem task)
        {
            SelectedItem = task;
            ShowWindow();
        }

        public void CloseWindow()
        {
            player.Stop();
            Visibility = Visibility.Hidden;
        }

        public void ShowWindow()
        {
            Visibility = Visibility.Visible;
            if (Config.Current.CanShake)
                ShakeWindow();
            if (Config.Current.CanSplash)
                SplashWindow();
        }

        public void SplashWindow()
        {
            try
            {
                if (SplashBoard != null)
                    SplashBoard.Begin();
            }
            catch (Exception ex)
            {
                Logger.Log("SplashWindow" + ex.Message);
            }
        }

        public void ShakeWindow()
        {
            try
            {
                Left = 0;
                Top = 0;
                if (board != null)
                    board.Begin();
            }
            catch (Exception ex)
            {
                Logger.Log("ShakeWindow" + ex.Message);
            }
        }

        private void board_Completed(object sender, EventArgs e)
        {
            Left = 0.0;
            Top = 0.0;
            UpdateLayout();
        }

        #region AlarmedTaskList

        public ObservableCollection<TaskItem> AlarmedTaskList
        {
            get { return (ObservableCollection<TaskItem>)GetValue(AlarmedTaskListProperty); }
            set { SetValue(AlarmedTaskListProperty, value); }
        }

        public static readonly DependencyProperty AlarmedTaskListProperty =
            DependencyProperty.Register("AlarmedTaskList", typeof(ObservableCollection<TaskItem>), typeof(ShowInfo),
                new PropertyMetadata(new ObservableCollection<TaskItem>(),
                    (sender, e) =>
                    {
                        var vm = sender as ShowInfo;
                        if (vm == null) return;
                        vm.BuidCollectionView();
                    }));

        #endregion

        #region SelectedItem

        public TaskItem SelectedItem
        {
            get { return (TaskItem)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(TaskItem), typeof(ShowInfo), new PropertyMetadata(
                (sender, e) =>
                {
                    var vm = sender as ShowInfo;
                    if (vm == null) return;
                    vm.PlaySound();
                }));

        #endregion

        #region CountItem

        public string CountItem
        {
            get { return (string)GetValue(CountItemProperty); }
            set { SetValue(CountItemProperty, value); }
        }

        public static readonly DependencyProperty CountItemProperty =
            DependencyProperty.Register("CountItem", typeof(string), typeof(ShowInfo), new PropertyMetadata(
                (sender, e) =>
                {
                    var vm = sender as ShowInfo;
                    if (vm == null) return;
                }));

        #endregion
    }
}