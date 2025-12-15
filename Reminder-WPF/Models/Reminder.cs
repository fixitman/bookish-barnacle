using System;

namespace Reminder_WPF.Models
{
    public class Reminder : IEquatable<Reminder>
    {
        public enum RecurrenceType { None, Daily, Weekly, Monthly }

        public static RecurrenceType[] GetRecurrenceTypes
        {
            get => Enum.GetValues<RecurrenceType>();
        }

        public int id {  get; set; }
        public string ReminderText { get; set; } = string.Empty;
        public DateTime ReminderTime { get; set; }
        public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;
        public string RecurrenceData { get; set; } = "";
        public long LastUpdated { get; set; }

        public bool Equals(Reminder? other)
        {
            //if( other == null ||
            //    id != other.id ||
            //    !ReminderText.Equals(other.ReminderText) ||
            //    !ReminderTime.Equals(other.ReminderTime) ||
            //    !Recurrence.Equals(other.Recurrence) ||
            //    !RecurrenceData.Equals(other.RecurrenceData))
            //{
            //    return false;
            //}
            if (other == null) return false;
            
            if(id != other.id) return false;
            if(!ReminderText.Equals(other.ReminderText)) return false;
            if(!Recurrence.Equals(other.Recurrence)) return false;
            if(!RecurrenceData.Equals(other.RecurrenceData)) return false;
            //if(!ReminderTime.Ticks.Equals(other.ReminderTime.Ticks)) return false;
            if(Recurrence == RecurrenceType.None)
            {
                if (!ReminderTime.Equals(other.ReminderTime)) return false;
            }
            else
            {
                if(!ReminderTime.TimeOfDay.Equals (other.ReminderTime.TimeOfDay)) return false;
            }
            return true;
        }
        public Reminder Clone()
        {
            return (Reminder) this.MemberwiseClone();
        }
    }
}