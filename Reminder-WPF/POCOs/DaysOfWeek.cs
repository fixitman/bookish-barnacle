using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder_WPF.POCOs
{
    class DaysOfWeek
    {
        public bool Sunday { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set;}
        public bool Friday      { get; set; }
        public bool Saturday { get;set; }

        public string Text { 
            get 
            { 
                string text = string.Empty;
                if(Sunday)
                {
                    if(text.Length > 0) { text += ","; }
                    text += "Sun";
                }
                if (Monday)
                {
                    if (text.Length > 0) { text += ","; }
                    text += "Mon";
                }
                if (Tuesday)
                {
                    if (text.Length > 0) { text += ","; }
                    text += "Tue";
                }
                if (Wednesday)
                {
                    if (text.Length > 0) { text += ","; }
                    text += "Wed";
                }
                if (Thursday)
                {
                    if (text.Length > 0) { text += ","; }
                    text += "Thu";
                }
                if (Friday)
                {
                    if (text.Length > 0) { text += ","; }
                    text += "Fri";
                }
                if (Saturday)
                {
                    if (text.Length > 0) { text += ","; }
                    text += "Sat";
                }

                return text;
            }
            set
            {
                value = value.Replace(" ",string.Empty).ToUpper();
                Sunday = value.Contains("SUN");
                Monday = value.Contains("MON");
                Tuesday = value.Contains("TUE");
                Wednesday = value.Contains("WED");
                Thursday = value.Contains("THU");
                Friday = value.Contains("FRI");
                Saturday = value.Contains("SAT");
            }
        }


    }
}
