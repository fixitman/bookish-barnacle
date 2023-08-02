﻿using System.Windows;
using System;
using Reminder_WPF.Models;
using System.Text.Json;

namespace Reminder_WPF.Views
{
    /// <summary>
    /// Interaction logic for AddEditReminderDlg.xaml
    /// </summary>
    public  partial class AddEditReminderDlg : Window
    {
        public enum DlgMode { Add, Edit};
        public Reminder Reminder { get; set; }

        public AddEditReminderDlg( Reminder? reminder = null)
        {
            InitializeComponent();
            if (reminder == null)
            {
                Reminder = new Reminder();
                Reminder.ReminderTime = DateTime.Now;
                Title = "Add Reminder";
            }
            else
            {
                var remCopy = JsonSerializer.Deserialize<Reminder>(JsonSerializer.Serialize<Reminder>(reminder));
                Reminder = remCopy!;
                Title = "Edit Reminder";
            }
            DataContext = Reminder;
            dtDate.Text = Reminder.ReminderTime.Date.ToString();
            txtTime.Text = string.Format(@"{0:hh\:mm}", Reminder.ReminderTime.TimeOfDay);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateOnly d;
            TimeOnly t;

            try
            {
                d = DateOnly.Parse(dtDate.Text);
                t = TimeOnly.Parse(txtTime.Text);
            }
            catch (Exception ex)
            {
                return;
            }
            var dt = new DateTime(d.Year, d.Month, d.Day, t.Hour, t.Minute, t.Second);
            Reminder.ReminderTime = dt;
            Reminder.ReminderText = txtReminderText.Text;
            DialogResult = true;
        }
            
       

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtReminderText.SelectAll();
            txtReminderText.Focus();
        }
    }
}