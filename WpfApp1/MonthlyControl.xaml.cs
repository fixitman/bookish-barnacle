using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp1
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
