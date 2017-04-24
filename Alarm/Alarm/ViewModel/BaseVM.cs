using System.Windows;

namespace Alarm.ViewModel
{
    public class BaseVM : DependencyObject
    {
        #region PageName

        private string pageName = string.Empty;

        public string PageName
        {
            get { return pageName; }
            set { pageName = value; }
        }

        #endregion

        #region Container
        public static readonly DependencyProperty ContainerProperty = DependencyProperty.Register(
            "Container", typeof(object), typeof(BaseVM), new PropertyMetadata((sender, e) =>
            {
                var vm = sender as BaseVM;
                if (vm == null) return;
            }));

        public object Container
        {
            get { return this.GetValue(ContainerProperty) as object; }
            set { this.SetValue(ContainerProperty, value); }
        }
        #endregion

        public BaseVM()
        {
            this.RegisterCommands();
        }

        public virtual void Loaded()
        {
        }

        public virtual void UnLoaded()
        {
            
        }

        public virtual void RegisterCommands()
        {
            
        }
    }
}