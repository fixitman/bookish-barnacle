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
            long delay = FindNext(r); ;

            Timer t = new Timer(this.SchedulerCallback, r, delay, Timeout.Infinite);
            ReminderEvent reminderEvent = new ReminderEvent { onTimer = callback, reminder = r, timer = t };
            

            Events.Add(r.id, reminderEvent);
            
        }

        private void SchedulerCallback(object? state)
        {
            Reminder? r = state as Reminder;
            if (r != null)
            {
                ReminderEvent reminderEvent = Events[r.id];
               
                if(r.Recurrence != Reminder.RecurrenceType.None)
                {
                    long delay = FindNext(r);                    
                    Events[r.id].timer.Change(delay, Timeout.Infinite);
                }
                
                reminderEvent.onTimer(reminderEvent.reminder);
            }
            else
            {
                Console.WriteLine("Event was passed a null reminder value");
            }
        }

        private long FindNext(Reminder r)
        {
            long delay;

            switch (r.Recurrence)
            {
                case Reminder.RecurrenceType.None:
                    delay = (long)(r.ReminderTime - DateTime.Now).TotalMilliseconds;
                    if (delay < 0) delay = Timeout.Infinite;                    
                    break;
                case Reminder.RecurrenceType.Daily:
                    delay = (long)(r.ReminderTime.TimeOfDay - DateTime.Now.TimeOfDay).TotalMilliseconds;
                    if (delay < 0) delay += 24*60*60*1000;
                    break;
                case Reminder.RecurrenceType.Weekly:
                    List<int> triggerDays = new List<int>();
                    List<string> daysAll = new List<string> { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

                    var daysText = r.RecurrenceData.Split(",");
                    foreach(string day in daysText)
                    {
                        triggerDays.Add(daysAll.FindIndex(d => d == day));
                    }
                   
                    DateTime trigger = r.ReminderTime;
                    var found = false;
                    while(trigger < DateTime.Now) trigger = trigger.AddDays(1);
                    while ( !found)
                    {                        
                        int dow = (int)trigger.DayOfWeek;
                        if (triggerDays.Contains(dow))
                        {
                            found = true;
                            Console.WriteLine("Next time is {0}", trigger);
                            break;
                        }
                        trigger = trigger.AddDays(1);
                        
                    }
                    delay = (long)(trigger - DateTime.Now).TotalMilliseconds;
                    break;
                default:
                    delay = (long)(r.ReminderTime - DateTime.Now).TotalMilliseconds;
                    break;
            }
            return delay;

        }

        public void SnoozeReminder(Reminder r, int minutes)
        {
            ReminderEvent? _event = Events[r.id];
            if (_event != null)
            {
                Timer t = _event.timer;
                long snoozeAmount = (long)(TimeSpan.FromMinutes(minutes).TotalMilliseconds);
                t.Change(snoozeAmount, Timeout.Infinite);
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
            Timer? timerToDelete = Events[r].timer;
            timerToDelete.Dispose();
            Events.Remove(r);
        }

        public void Dispose()
        {
            foreach (int key in Events.Keys)
            {
                ReminderEvent wrapper = Events[key];
                Console.WriteLine("Timer {0:HH:mm:ss} disposed. ",wrapper.timer);
                wrapper.timer.Dispose();
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
