using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Reminder_WPF.Views
{
    /// <summary>
    /// Interaction logic for MonthlyControl.xaml
    /// </summary>
    public partial class MonthlyControl : UserControl
    {
        public List<KeyValuePair<int, string>> Modes { get; set; } 
        public List<KeyValuePair<int, string>> Nths { get; set; } 
        public List<KeyValuePair<int, string>> DaysOfWeek { get; set; } 
        public int SelectedMode { get; set; } 
        public int DayOfMonth {  get; set; }
        public int SelectedNth {  get; set; }
        public int SelectedDayOfWeek { get; set; }

        public string Text
        {
            get
            {
                string text = string.Empty;
                switch (SelectedMode)
                {
                    case 0:
                        text = string.Format("{0},{1}",SelectedMode,DayOfMonth);
                        break;
                    case 1:
                        text = string.Format("{0},{1},{2}",SelectedMode,SelectedNth,SelectedDayOfWeek);
                        break;
                    default:
                        break;

                }
                return text;
                
            }

            set
            {
                var data = value.Split(',');
                switch (data[0])
                {
                    case "0":
                        SelectedMode = 0;
                        DayOfMonth = Convert.ToInt32(data[1]);
                        break;
                    case "1":
                        SelectedMode = 1;
                        SelectedNth = Convert.ToInt32(data[1]);
                        SelectedDayOfWeek = Convert.ToInt32(data[2]);
                        break;
                    default:
                        break;

                }
            }
        }

        public MonthlyControl()
        {
            Modes = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(0, "By Date"),
                new KeyValuePair<int, string>(1, "Nth Day")
            };

            Nths = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(0, "1st"),
                new KeyValuePair<int, string>(1, "2nd"),
                new KeyValuePair<int, string>(2, "3rd"),
                new KeyValuePair<int, string>(3, "4th")
            };
            DaysOfWeek = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(0, "Sunday"),
                new KeyValuePair<int, string>(1, "Monday"),
                new KeyValuePair<int, string>(2, "Tuesday"),
                new KeyValuePair<int, string>(3, "Wednesday"),
                new KeyValuePair<int, string>(4, "Thursday"),
                new KeyValuePair<int, string>(5, "Friday"),
                new KeyValuePair<int, string>(6, "Saturday")
            };
            

            SelectedMode = 0;
            DayOfMonth = 1;
            SelectedDayOfWeek = 0;
            SelectedNth = 0;

            InitializeComponent();
            DataContext = this;
            
        }

        private void CBMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(SelectedMode)
            {
                case 0: //By Date
                    stkDayOfMonth.Visibility = Visibility.Visible;
                    stkNth.Visibility = Visibility.Collapsed;
                    break;
                case 1:  //Nth Day
                    stkDayOfMonth.Visibility = Visibility.Collapsed;                    
                    stkNth.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsDigit(e.Text);
        }

        private bool IsDigit(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            Regex regex = new Regex("[0-9]");
            return regex.IsMatch(text);            
        }
    }
}
