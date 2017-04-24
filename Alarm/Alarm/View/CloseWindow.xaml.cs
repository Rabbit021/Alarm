using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Alarm.Models;
using System.Security;
using Alarm.Common;

namespace Alarm.View
{
    /// <summary>
    /// CloseWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CloseWindow : Window
    {
        public CloseWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            try
            {
                var sec = this.pwd.Password;
                var pass = Config.Current.Password;
                if (string.Equals(sec, pass))
                {
                    Application.Current.Shutdown();
                }
            }
            catch(Exception exp)
            {
                Logger.Log(exp.Message);
                Environment.Exit(0);
            }
        }


        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}