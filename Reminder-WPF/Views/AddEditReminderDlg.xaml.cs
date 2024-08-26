using CommunityToolkit.Mvvm.ComponentModel;
using Reminder_WPF.Models;
using Reminder_WPF.Services;
using System;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace Reminder_WPF.Views
{

    [ObservableObject]
    public  partial class AddEditReminderDlg : Window, IDisposable
    {
        public Reminder Reminder { get; set; }
        
        [ObservableProperty]
        private string errorMessage = "";
        [ObservableProperty]
        private DateTime selectedDate; 
        [ObservableProperty]
        private string txtTime;

        public AddEditReminderDlg(Reminder? reminder = null)
        {
            InitializeComponent();
            if (reminder == null)
            {
                Reminder = new Reminder();
                Reminder.ReminderTime = DateTime.Now.AddMinutes(10);
                Title = "Add Reminder";
            }
            else
            {
                var remCopy = JsonSerializer.Deserialize<Reminder>(JsonSerializer.Serialize<Reminder>(reminder));
                Reminder = remCopy!;
                Title = "Edit Reminder";
            }
            DataContext = Reminder;
            SelectedDate = Reminder.ReminderTime.Date;
            TxtTime = Reminder.ReminderTime.ToShortTimeString();
            txt_Time.GotKeyboardFocus += SelectAallOnKeyboardFocus;
            txtReminderText.GotKeyboardFocus += SelectAallOnKeyboardFocus;
            
            txtError.DataContext = this;
            dtDate.DataContext = this;
            txt_Time.DataContext = this;
        }

        private void SelectAallOnKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            TextBox? t = e.OriginalSource as TextBox;
            t?.SelectAll();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            if(txtReminderText.Text.Length < 1)
            {
                ErrorMessage = "Reminder Text Required";
                txtReminderText.Focus();
                return;
            }                  
            if (Reminder.Recurrence == Reminder.RecurrenceType.Weekly && string.IsNullOrEmpty(WeeklyControl.Text))
            {
                ErrorMessage = "You must select at least one day";
                return;
            }            
            UpdateReminderTime();
            GetRecurrenceData();
            DialogResult = true;
        }          
       
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtReminderText.SelectAll();
            txtReminderText.Focus();
        }
                
        internal void ClearErrorMessage()
        {
            ErrorMessage = "";
        }

        private void DataChanged(object sender, object e)
        {
            ClearErrorMessage();            
        }

        partial void OnErrorMessageChanged(string value)
        {
            txtError.Visibility = value.Length > 0? Visibility.Visible : Visibility.Collapsed;
            
        }

        private void Recurrence_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var r = (sender as ComboBox)?.SelectedItem.ToString();
            switch (r?.ToUpper())
            {

                case "WEEKLY":
                    RecurrenceDataHolder.Visibility = Visibility.Visible;
                    WeeklyControl.Visibility = Visibility.Visible;
                    Monthly_Control.Visibility = Visibility.Collapsed;
                    break;

                case "MONTHLY":
                    RecurrenceDataHolder.Visibility = Visibility.Visible;
                    WeeklyControl.Visibility = Visibility.Collapsed;
                    Monthly_Control.Visibility= Visibility.Visible;
                    break;
                    

                default:
                    RecurrenceDataHolder.Visibility = Visibility.Collapsed;
                    WeeklyControl.Visibility = Visibility.Collapsed;
                    Monthly_Control.Visibility = Visibility.Collapsed;
                    break;
            }

            PopulateRecurrenceControls();

        }

        private void PopulateRecurrenceControls()
        {
            if(Reminder.Recurrence == Reminder.RecurrenceType.Weekly)
            {
                WeeklyControl.Text = Reminder.RecurrenceData;
            }
            else if(Reminder.Recurrence == Reminder.RecurrenceType.Monthly)
            {
                Monthly_Control.Text = Reminder.RecurrenceData;
            }
        }

        private void GetRecurrenceData()
        {
            if(Reminder.Recurrence == Reminder.RecurrenceType.Weekly)
            {
                Reminder.RecurrenceData = WeeklyControl.Text;                
            }
            else if(Reminder.Recurrence == Reminder.RecurrenceType.Monthly)
            {
                Reminder.RecurrenceData = Monthly_Control.Text;
            }
        }

        private void UpdateReminderTime()
        {
            TimeSpan t;
            try
            {
                if (TxtTime != null )
                {
                    t = DateTime.Parse(TxtTime).TimeOfDay;
                    var wholeThing = SelectedDate + t;
                    Reminder.ReminderTime = wholeThing;
                }
            }
            catch (Exception)
            {
                ErrorMessage = "Invalid Date or Time";
                return;
            }
            
            
           
        }

        partial void OnTxtTimeChanged( string value)
        {
            UpdateReminderTime();
           
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            UpdateReminderTime();
            
        }

        public void Dispose()
        {
            txt_Time.GotKeyboardFocus -= SelectAallOnKeyboardFocus;
            txtReminderText.GotKeyboardFocus -= SelectAallOnKeyboardFocus;
        }

    }
}
