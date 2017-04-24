using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Alarm.ViewModel;

namespace Alarm.View
{
    /// <summary>
    ///     TaskDetail.xaml 的交互逻辑
    /// </summary>
    public partial class TaskDetail : Window
    {
        public TaskDetail()
        {
            InitializeComponent();
            Loaded += TaskDetail_Loaded;

            player.MediaEnded -= player_MediaEnded;
            player.MediaEnded += player_MediaEnded;
        }

        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            //this.player.Position = TimeSpan.Zero; 
        }

        private void TaskDetail_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = TaskDetailVM.Instance;
            TaskDetailVM.Instance.Loaded();
        }

        private void bg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
            TaskDetailVM.Instance.Cancle();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // 在此处添加事件处理程序实现。
            TaskDetailVM.Instance.Save();
            CloseWindow();
        }

        public void CloseWindow()
        {
            player.Stop();
            Close();
            Visibility = Visibility.Hidden;
        }

        public void ShowWindow()
        {
            player.Play();
            ShowDialog();
//            Visibility = Visibility.Visible;
        }

        private void sdVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Play();
        }

        private void cboAudio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            player.Play();
        }
    }
}