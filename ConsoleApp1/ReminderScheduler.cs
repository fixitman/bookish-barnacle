namespace ConsoleApp1
{
    internal class ReminderScheduler
    {
        private Dictionary<int,Timer> Timers  { get; set; }

        public ReminderScheduler()
        {
            Timers = new Dictionary<int,Timer>();
        }


        public void ScheduleReminder(Reminder r, TimerCallback  callback )
        {
            //long 
            long delay = r.Recurrence != Reminder.RecurrenceType.None ? FindNext(r) : (long)(r.ReminderTime - DateTime.Now).TotalMilliseconds;

            Timer t = new Timer(callback, r, delay, Timeout.Infinite);
            Timers.Add(r.id, t);
        }

        private long FindNext(Reminder r)
        {
            long delay;
            

            switch (r.Recurrence)
            {
                
                case Reminder.RecurrenceType.None:
                    delay = (long)(r.ReminderTime - DateTime.Now).TotalMilliseconds;
                    break;
                case Reminder.RecurrenceType.Daily:
                    delay = (long)(r.ReminderTime - DateTime.Now).TotalMilliseconds;
                    break;
                case Reminder.RecurrenceType.Weekly:
                    List<int> daysNum = new List<int>();
                    List<string> daysAll = new List<string> { "sun", "mon", "tue", "wed", "thu", "fri", "sat" };

                    var daysText = r.RecurrenceData.Split(",");
                    foreach(string day in daysText)
                    {
                        daysNum.Add(daysAll.FindIndex(d => d == day));
                    }
                   
                    DateTime date = r.ReminderTime;
                    var found = false;
                    while(date < DateTime.Now) date = date.AddDays(1);
                    while ( !found)
                    {                        
                        int dow = (int)date.DayOfWeek;
                        if (daysNum.Contains(dow))
                        {
                            found = true;
                            Console.WriteLine("Next time is {0}", date);
                            break;
                        }
                        date = date.AddDays(1);
                        
                    }
                    delay = (long)(date - DateTime.Now).TotalMilliseconds;
                    break;
                default:
                    delay = (long)(r.ReminderTime - DateTime.Now).TotalMilliseconds;
                    break;
            }
            return delay;

        }

        public void UpdateReminder(Reminder r)
        {
            if( Timers.ContainsKey(r.id))
            {
                long delay = r.Recurrence != Reminder.RecurrenceType.None? FindNext(r) : (long)(r.ReminderTime - DateTime.Now).TotalMilliseconds;
                Timers[r.id].Change(delay, Timeout.Infinite);
            }
        }

        public void DeleteReminder(int r)
        {
            var timerToDelete = Timers[r];
            timerToDelete.Dispose();
            Timers.Remove(r);
        }

        

    }
}
