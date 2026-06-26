using Reminder_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Reminder_WPF.Services
{
    public class ReminderScheduler: IDisposable
    {
        private Dictionary<string,ReminderEvent> Events  { get; set; }

        public ReminderScheduler()
        {
            Events = new Dictionary<string,ReminderEvent>();
        }


        public void ScheduleReminder(Reminder r, TimerCallback  callback )
        {
            long delay = (long)(FindNext(r) - DateTime.Now).TotalMilliseconds  ;
            if (delay == Timeout.Infinite || delay >= Int32.MaxValue - 2) return;

            Timer t = new Timer(this.SchedulerCallback, r, delay, Timeout.Infinite);
            ReminderEvent reminderEvent = new ReminderEvent { onTimer = callback, reminder = r, timer = t };
            if (Events.ContainsKey(r.id))
            {
                DeleteReminder(r.id);                
            }
            Events.Add(r.id, reminderEvent);
            
        }

        private void SchedulerCallback(object? state)
        {
            Reminder? r = state as Reminder;
            if (r != null)
            {
                if (!Events.ContainsKey(r.id)) return;

                ReminderEvent reminderEvent = Events[r.id];
                
                reminderEvent.onTimer(reminderEvent.reminder);
            }
            else
            {
                Console.WriteLine("Event was passed a null reminder value");
            }
        }
         
        internal void ScheduleNext(Reminder r)
        {
            if (!Events.ContainsKey(r.id)) return;
            ReminderEvent reminderEvent = Events[r.id];
            long delay = (long)(FindNext(r) - DateTime.Now).TotalMilliseconds;
            reminderEvent.timer.Change(delay, Timeout.Infinite);
            
        }

        public void SnoozeReminder(Reminder r, int minutes)
        {
            if(Events.ContainsKey(r.id))
            {
                ReminderEvent? _event = Events[r.id];
                if (_event != null)
                {
                    Timer t = _event.timer;
                    long snoozeAmount = (long)(TimeSpan.FromMinutes(minutes).TotalMilliseconds);
                    t.Change(snoozeAmount, Timeout.Infinite);
                }
            }
        }

        public void UpdateReminder(Reminder r)
        {
            if( Events.ContainsKey(r.id))
            {
                long delay = (long)(FindNext(r) - DateTime.Now).TotalMilliseconds;
                Events[r.id].timer.Change(delay, Timeout.Infinite);
            }
        }

        public void DeleteReminder(string r)
        {
            if (Events.ContainsKey(r))
            {
                Timer? timerToDelete = Events[r].timer;
                if (timerToDelete != null)
                {
                    timerToDelete.Dispose();
                    Events.Remove(r);
                }
            }
        }

        public DateTime GetNext(Reminder r)
        {
            var n = FindNext(r).AddMilliseconds(10);
            return new DateTime(n.Year, n.Month, n.Day, n.Hour, n.Minute, n.Second);
        }


        public DateTime FindNext(Reminder r)
        {
            DateTime next;

            switch (r.Recurrence)
            {
                case Reminder.RecurrenceType.None:
                    next = r.ReminderTime;                 
                    break;
                case Reminder.RecurrenceType.Daily:
                    next = r.ReminderTime;
                    if (next <= DateTime.Now)
                    {
                        next = next.AddHours(24);
                    }
                    break;
                case Reminder.RecurrenceType.Weekly:
                    next = FindNextWeekly(r);                    
                    break;
                case Reminder.RecurrenceType.Monthly:
                    var trigger1 = FindNextMonthly(r);
                    next = trigger1;
                    break;
                default:
                    next = r.ReminderTime;
                    break;
            }
            return next;

        }

        private static DateTime FindNextWeekly(Reminder r)
        {
            List<int> triggerDays = new List<int>();
            List<string> daysAll = new List<string> { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

            var daysText = r.RecurrenceData.Split(",");
            foreach (string day in daysText)
            {
                triggerDays.Add(daysAll.FindIndex(d => d == day));
            }

            DateTime trigger = r.ReminderTime;
            var found = false;
            while (trigger < DateTime.Now) trigger = trigger.AddDays(1);
            while (!found)
            {
                int dow = (int)trigger.DayOfWeek;
                if (triggerDays.Contains(dow))
                {
                    found = true;                    
                    break;
                }
                trigger = trigger.AddDays(1);
            }

            return trigger;
        }

        private DateTime FindNextMonthly(Reminder r)
        {
            var recurrenceData = r.RecurrenceData.Split(",");
            DateTime nthDate;
            DateTime trigger = DateTime.Now;
            if (recurrenceData[0] == "0")
            {
                int dayOfMonth = Int32.Parse(recurrenceData[1]);
                trigger = new DateTime(DateTime.Now.Year, DateTime.Now.Month, dayOfMonth, r.ReminderTime.Hour, r.ReminderTime.Minute, 0);
                if (trigger <= DateTime.Now)
                {
                    trigger = trigger.AddMonths(1);
                }
            }
            if (recurrenceData[0] == "1") // nth day
            {                
                DayOfWeek dayOfWeek = (DayOfWeek)Int32.Parse(recurrenceData[2]);
                nthDate = FindNthDayOfMonth(r.ReminderTime, Int32.Parse(recurrenceData[1]), dayOfWeek);
                while (nthDate <= DateTime.Now)
                {
                    nthDate = FindNthDayOfMonth(nthDate.AddMonths(1), Int32.Parse(recurrenceData[1]), dayOfWeek);
                }
                trigger = new DateTime(nthDate.Year, nthDate.Month, nthDate.Day, r.ReminderTime.Hour, r.ReminderTime.Minute, 0);
            }

            return trigger;
        }

        private DateTime FindNthDayOfMonth(DateTime monthToFind, int n, DayOfWeek day)
        {
            DateTime first = monthToFind.AddDays(-(monthToFind.Day - 1));
            while(first.DayOfWeek != day)
            {
                first = first.AddDays(1);
            }
            DateTime nth = first.AddDays(7*(n));
            return nth;
        }

        public void ClearEvents()
        {
            foreach (var key in Events.Keys)
            {
                Events[key].timer.Dispose();
                Events.Remove(key);
            }
        }

        public void Dispose()
        {
            foreach (string key in Events.Keys)
            {
                ReminderEvent _event = Events[key];                
                _event.timer.Dispose();
            }
        }

        class ReminderEvent
        {
            public required Reminder reminder { get; set; }
            public required Timer timer { get; set; }
            public required TimerCallback onTimer { get; set; }
        }
    }
}
