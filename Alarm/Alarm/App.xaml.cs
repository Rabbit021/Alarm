using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace Alarm
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //初始化
            base.OnStartup(e);
            System.Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
        }
    }
}
