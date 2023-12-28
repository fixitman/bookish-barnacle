using Reminder_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public partial class NotificationWindow : Window
    {
        public Reminder Reminder { get; }
        public bool Snoozed { get; private set; } = false;

        public NotificationWindow(Reminder reminder)
        {
            InitializeComponent();
            Reminder = reminder;            
        }

        private void Dismiss_Click(object sender, RoutedEventArgs e)
        {
            Snoozed = false;
            this.DialogResult = true;

        }

        private void Snooze_Click(object sender, RoutedEventArgs e)
        {
            Snoozed = true;
            this.DialogResult = true;
        }
    }
}
