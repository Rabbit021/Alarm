using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Alarm
{
    /// <summary>
    ///     TimeClockControl.xaml 的交互逻辑
    /// </summary>
    public partial class TimeClockControl : UserControl
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();

        public TimeClockControl()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += TimeClockControl_Loaded;
        }

        private void TimeClockControl_Loaded(object sender, RoutedEventArgs e)
        {
            CreateNowTimeString();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            CreateNowTimeString();
        }

        private void CreateNowTimeString()
        {
            NowTime = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
        }

        #region NowTime

        public string NowTime
        {
            get { return (string) GetValue(NowTimeProperty); }
            set { SetValue(NowTimeProperty, value); }
        }

        public static readonly DependencyProperty NowTimeProperty =
            DependencyProperty.Register("NowTime", typeof (string), typeof (TimeClockControl), new PropertyMetadata(
                (sender, e) =>
                {
                    var vm = sender as TimeClockControl;
                    if (vm == null) return;
                }));

        #endregion
    }
}