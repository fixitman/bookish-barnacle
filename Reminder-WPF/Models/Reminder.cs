using System;

namespace Reminder_WPF.Models
{
    public class Reminder
    {
        public int id {  get; set; }
        public string ReminderText { get; set; }
        public DateTime ReminderTime { get; set; }
    }
}