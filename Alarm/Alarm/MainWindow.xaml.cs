using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Alarm.Models;
using Alarm.View;
using Alarm.ViewModel;
using Microsoft.Expression.Interactivity.Core;

namespace Alarm
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly bool _reallyExit = false; //是否真的关闭窗口  
        private WindowState _lastWinState = WindowState.Normal; //记录上一次WindowState 

        public MainWindow()
        {
            InitializeComponent();
            //            Visibility = Visibility.Hidden;
            Instance = this;
            DataContext = MainPageVM.Instance;
            RegisterEvent();
        }

        public static MainWindow Instance { get; private set; }
        private bool MinToTray { get; set; }

        private void Init()
        {
        }

        private void RegisterEvent()
        {
            Loaded += MainWindow_Loaded;
            MouseLeftButtonDown += Window_MouseLeftButtonDown;
            PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                var info = new ShowInfo();
                info.Show();
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainPageVM.Instance.Loaded();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //            var detail = new TaskDetail();
            //            detail.ShowDialog();
        }

        #region 窗体

        /// <summary>
        ///     窗体移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        ///     窗体最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            Minimized();
        }

        /// <summary>
        ///     窗口最小化
        /// </summary>
        private void Minimized()
        {
            WindowState = WindowState.Minimized;
            Visibility = Visibility.Hidden; //最小化到托盘
            miShowWindow.Header = "显示窗口";
        }

        /// <summary>
        ///     窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            //            if (MessageBox.Show("确定要退出托盘应用程序吗？",
            //                "托盘应用程序",
            //                MessageBoxButton.YesNo,
            //                MessageBoxImage.Question,
            //                MessageBoxResult.No) == MessageBoxResult.Yes)
            //            {
            //                Application.Current.Shutdown();
            //                tbIcon.Visibility = Visibility.Hidden;
            //            }
            var close = new CloseWindow();
            close.ShowDialog();
        }

        #endregion

        #region 托盘相关

        /// <summary>
        ///     菜单项"显示主窗口"点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miShow_Click(object sender, RoutedEventArgs e)
        {
            if (IsVisible)
            {
                Hide();
                miShowWindow.Header = "显示窗口";
            }
            else
            {
                Show();
                miShowWindow.Header = "隐藏窗口";
            }
            if (WindowState == WindowState.Minimized)
            {
                WindowState = _lastWinState;
            }
            Activate();
        }

        //托盘
        private void Window_Close(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        /// <summary>
        ///     关闭时,判断是缩小到托盘还是退出程序
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!_reallyExit)
            {
                e.Cancel = true;
                _lastWinState = WindowState;
                Hide();
            }
            tbIcon.Dispose();
        }

        #endregion
    }

    public class MainPageVM : DependencyObject
    {
        private readonly List<ShowInfo> ShowInfoWindows = new List<ShowInfo>();
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private ShowInfo showWiondow;

        private MainPageVM()
        {
            RegisterCommands();
        }

        public void Loaded()
        {
            Init();
            // 定时任务
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void Init()
        {
            var vmList = new ObservableCollection<BaseVM>();
            var viewList = new ObservableCollection<UserControl>();
            {
                var page = new TaskList();
                page.DataContext = TaskListVM.Instance;
                viewList.Add(page);
                vmList.Add(TaskListVM.Instance);
            }

            CenterVMs = vmList;
            CenterViews = viewList;
            ChangePage("TaskList");
            LoadTask.Reload();
            showWiondow = new ShowInfo();
        }

        private void ChangePage(string pageName)
        {
            try
            {
                var page = GetPage(pageName);
                if (page == null) return;
                if (CurrentView == page) return;
                CurrentView = page;
                CurrentVM = page.DataContext as BaseVM;
            }
            catch (Exception exp)
            {
            }
        }

        private UserControl GetPage(string pageName)
        {
            try
            {
                var page = CenterViews.FirstOrDefault(x => x.DataContext as BaseVM != null &&
                                                           string.Equals((x.DataContext as BaseVM).PageName, pageName,
                                                               StringComparison.CurrentCultureIgnoreCase));
                return page;
            }
            catch
            {
                return null;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var currentDate = DateTime.Now.ToShortDateString();
            var tasks = LoadTask.AllTask;
            if (tasks == null) return;
            foreach (var task in tasks)
            {
                var isTimeEqual = TimeCheck(DateTime.Now, DateTime.Parse(task.Time));
                if (!isTimeEqual) continue;
                switch (task.ItemType)
                {
                    case TaskType.Once:
                        if (task.Date == currentDate)
                            task.State = TaskState.Alarmed;
                        break;
                    case TaskType.EveryDay:
                        task.State = TaskState.Alarmed;
                        break;
                    case TaskType.EveryWeek:
                        var check = task.WeekDays.First(x => x.Week == now.DayOfWeek).Checked;
                        if (check)
                            task.State = TaskState.Alarmed;
                        break;
                    case TaskType.EveryMonth:
                        var res = now.Day == DateTime.Parse(task.Date).Day;
                        if (res)
                            task.State = TaskState.Alarmed;
                        break;
                }
                CheckState(task);
            }
            if (showWiondow != null)
                showWiondow.CheckAlarm(tasks);
        }

        private bool TimeCheck(DateTime nowTime, DateTime destTime)
        {
            var span = destTime - nowTime;
            return nowTime.Hour == destTime.Hour && nowTime.Minute == destTime.Minute && span.Milliseconds <= 1000;
        }

        public void CheckState(TaskItem task)
        {
            if (task.State == TaskState.WaitDeal)
            {
                var span = DateTime.Now - DateTime.Parse(task.Time);
                if (span.TotalSeconds > Config.Current.CloseAlarmSpan)
                    TaskManager.Instance.DealTask(task);
            }
        }

        private void RegisterCommands()
        {
            AddTaskCommand = new ActionCommand(AddTask);
            EditTaskCommand = new ActionCommand(EditTask);
            DelTaskCommand = new ActionCommand(DelTask);
        }

        private void DelTask(object obj)
        {
            var detail = new TaskDetail();
            detail.ShowDialog();
        }

        private void EditTask(object obj)
        {
            var detail = new TaskDetail();
            detail.ShowDialog();
        }

        private void AddTask(object obj)
        {
            var detail = new TaskDetail();
            detail.ShowDialog();
        }

        #region Instance

        private static readonly MainPageVM instance = new MainPageVM();

        public static MainPageVM Instance
        {
            get { return instance; }
        }

        #endregion

        #region CenterViews

        public ObservableCollection<UserControl> CenterViews
        {
            get { return (ObservableCollection<UserControl>)GetValue(CenterViewsProperty); }
            set { SetValue(CenterViewsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CenterViews.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CenterViewsProperty =
            DependencyProperty.Register("CenterViews", typeof(ObservableCollection<UserControl>), typeof(MainPageVM),
                new PropertyMetadata(
                    (sender, e) =>
                    {
                        var vm = sender as MainPageVM;
                        if (vm == null) return;
                    }));

        #endregion

        #region CenterVMs

        public ObservableCollection<BaseVM> CenterVMs
        {
            get { return (ObservableCollection<BaseVM>)GetValue(CenterVMsProperty); }
            set { SetValue(CenterVMsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentViewModels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CenterVMsProperty =
            DependencyProperty.Register("CenterVMs", typeof(ObservableCollection<BaseVM>),
                typeof(MainPageVM), new PropertyMetadata((sender, e) =>
                {
                    var vm = sender as MainPageVM;
                    if (vm == null) return;
                }));

        #endregion

        #region CurrentVM

        public BaseVM CurrentVM
        {
            get { return (BaseVM)GetValue(CurrentVMProperty); }
            set { SetValue(CurrentVMProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentVM.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentVMProperty =
            DependencyProperty.Register("CurrentVM", typeof(BaseVM), typeof(MainPageVM), new PropertyMetadata(
                (sender, e) =>
                {
                    var vm = sender as MainPageVM;
                    if (vm == null) return;
                    var old = e.OldValue as BaseVM;
                    if (old != null) old.UnLoaded();
                    var nvm = e.NewValue as BaseVM;
                    nvm.Loaded();
                }));

        #endregion

        #region CurrentView

        public UserControl CurrentView
        {
            get { return (UserControl)GetValue(CurrentViewProperty); }
            set { SetValue(CurrentViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentViewProperty =
            DependencyProperty.Register("CurrentView", typeof(UserControl), typeof(MainPageVM), new PropertyMetadata(
                (sender, e) =>
                {
                    var vm = sender as MainPageVM;
                    if (vm == null) return;
                }));

        #endregion

        #region Command

        public ICommand AddTaskCommand { get; set; }
        public ICommand EditTaskCommand { get; set; }
        public ICommand DelTaskCommand { get; set; }

        #endregion

        #region TaskManager

        #endregion
    }
}