using CommunityToolkit.Mvvm.ComponentModel;
using Reminder_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Reminder_WPF.Views
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    [ObservableObject]
    public partial class NotificationWindow : Window
    {
        [ObservableProperty]
        private string reminderText;
        public bool WasSnoozed { get; private set; } = false;
        public string SnoozeMinutes { get; set; }

        public NotificationWindow(string reminderText)
        {
            InitializeComponent();
            DataContext = this;
            ReminderText = reminderText;
            SnoozeMinutes = "10";
        }

        private void Dismiss_Click(object sender, RoutedEventArgs e)
        {
            WasSnoozed = false;
            this.DialogResult = true;

        }

        private void Snooze_Click(object sender, RoutedEventArgs e)
        {
            WasSnoozed = true;
            this.DialogResult = true;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            this.Activate();
        }
    }
}
