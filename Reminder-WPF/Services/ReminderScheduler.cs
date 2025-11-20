using Reminder_WPF.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Reminder_WPF.Services
{
    public class ReminderScheduler: IDisposable
    {
        private Dictionary<int,ReminderEvent> Events  { get; set; }

        public ReminderScheduler()
        {
            Events = new Dictionary<int,ReminderEvent>();
        }


        public void ScheduleReminder(Reminder r, TimerCallback  callback )
        {
            long delay = FindNext(r);
            if (delay == Timeout.Infinite || delay >= Int32.MaxValue - 2) return;

            Timer t = new Timer(this.SchedulerCallback, r, delay, Timeout.Infinite);
            ReminderEvent reminderEvent = new ReminderEvent { onTimer = callback, reminder = r, timer = t };
            if (Events.ContainsKey(r.id))
            {
                Events.Remove(r.id);
            }
            Events.Add(r.id, reminderEvent);
            
        }

        private void SchedulerCallback(object? state)
        {
            Reminder? r = state as Reminder;
            if (r != null)
            {
                ReminderEvent reminderEvent = Events[r.id];
               
                //if(reminderEvent.reminder.Recurrence != Reminder.RecurrenceType.None)
                //{
                //    long delay = FindNext(r);                    
                //    reminderEvent.timer.Change(delay, Timeout.Infinite);
                //}                
                
                reminderEvent.onTimer(reminderEvent.reminder);
            }
            else
            {
                Console.WriteLine("Event was passed a null reminder value");
            }
        }
         
        internal void ScheduleNext(Reminder r)
        {
            ReminderEvent reminderEvent = Events[r.id];
            long delay = FindNext(r);
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
                long delay = r.Recurrence != Reminder.RecurrenceType.None? FindNext(r) : (long)(r.ReminderTime - DateTime.Now).TotalMilliseconds;
                Events[r.id].timer.Change(delay, Timeout.Infinite);
            }
        }

        public void DeleteReminder(int r)
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

        public long FindNext(Reminder r)
        {
            long delay = -1L;

            switch (r.Recurrence)
            {
                case Reminder.RecurrenceType.None:
                    delay = (long)(r.ReminderTime - DateTime.Now).TotalMilliseconds;
                    if (delay < 0) delay = Timeout.Infinite;                    
                    break;
                case Reminder.RecurrenceType.Daily:
                    delay = (long)(r.ReminderTime.TimeOfDay - DateTime.Now.TimeOfDay).TotalMilliseconds;
                    if (delay < 0) delay += (long)TimeSpan.FromDays(1).TotalMilliseconds;
                    break;
                case Reminder.RecurrenceType.Weekly:
                    DateTime trigger = FindNextWeekly(r);
                    delay = (long)(trigger - DateTime.Now).TotalMilliseconds;
                    break;
                case Reminder.RecurrenceType.Monthly:
                    var trigger1 = FindNextMonthly(r);
                    delay = (long)(trigger1 - DateTime.Now).TotalMilliseconds;
                    break;
                default:
                    delay = (long)(r.ReminderTime - DateTime.Now).TotalMilliseconds;
                    break;
            }
            return delay;

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
            foreach (int key in Events.Keys)
            {
                ReminderEvent _event = Events[key];                
                _event.timer.Dispose();
            }
        }

        class ReminderEvent
        {
            public Reminder reminder { get; set; }
            public Timer timer { get; set; }
            public TimerCallback onTimer { get; set; }
        }
    }
}
