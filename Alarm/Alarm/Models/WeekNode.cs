using System;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Alarm.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class WeekNode : DependencyObject
    {
        public WeekNode()
        {
            Checked = false;
        }

        #region Name
        [JsonProperty]
        public string Name
        {
            get { return (string) GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof (string), typeof (WeekNode),
                new PropertyMetadata((sender, e) => { }));

        #endregion

        [JsonProperty]
        public DayOfWeek Week { get; set; } 

        #region Checked
        [JsonProperty]
        public bool Checked
        {
            get { return (bool) GetValue(CheckedProperty); }
            set { SetValue(CheckedProperty, value); }
        }

        public static readonly DependencyProperty CheckedProperty =
            DependencyProperty.Register("Checked", typeof (bool), typeof (WeekNode),
                new PropertyMetadata((sender, e) => { }));

        #endregion
    }
}