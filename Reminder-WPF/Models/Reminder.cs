using System;

namespace Reminder_WPF.Models
{
    public class Reminder
    {
        public enum RecurrenceType { None, Daily, Weekly }

        public static RecurrenceType[] GetRecurrenceTypes
        {
            get => Enum.GetValues<RecurrenceType>();
        }

        public int id {  get; set; }
        public string ReminderText { get; set; }
        public DateTime ReminderTime { get; set; }
        public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;
        public string RecurrenceData { get; set; } = "";

    }
}