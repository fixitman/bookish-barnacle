using System.Windows;
using System;
using Reminder_WPF.Models;
using System.Text.Json;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Reminder_WPF.Views
{

    [ObservableObject]
    public  partial class AddEditReminderDlg : Window
    {
        public Reminder Reminder { get; set; }
        
        [ObservableProperty]
        private string errorMessage = "";

        public AddEditReminderDlg( Reminder? reminder = null)
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
            txtError.DataContext = this;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateOnly d;
            TimeOnly t;
            if(txtReminderText.Text.Length < 1)
            {
                ErrorMessage = "Reminder Text Required";
                txtReminderText.Focus();
                return;
            }
            try
            {
                d = DateOnly.Parse(dtDate.Text);
                t = TimeOnly.Parse(txtTime.Text);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Invalid Date or Time";
                return;
            }
            var dt = new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, t.Second);
            Reminder.ReminderTime = dt;
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
    }
}
