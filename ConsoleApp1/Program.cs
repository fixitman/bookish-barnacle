using System.Data;
using System.Net.Http.Json;

namespace ConsoleApp1
{
    internal class Program
        {

        private static ReminderScheduler scheduler = new ReminderScheduler();

        static void Main(string[] args)
        {
            
                Reminder r = new Reminder
                {
                    id = 1,
                    ReminderTime = DateTime.Now.AddSeconds(2),
                    ReminderText = "You have been reminded!",
                    Recurrence = Reminder.RecurrenceType.None,
                    RecurrenceData = "mon,fri"
                };

            //scheduler.ScheduleReminder(r, Callback);
            //Thread.Sleep(1000);
            //r.ReminderText += " Changed!";
            //r.ReminderTime = DateTime.Now.AddSeconds(9);
            //scheduler.UpdateReminder(r);

            //
            Console.WriteLine( (FindNthDayOfMonth(DateTime.Now.AddMonths(1), 3, DayOfWeek.Sunday).ToString("D")));
            
            Console.ReadLine();   //to keep the program running
            scheduler.Dispose();
        }

        private static DateTime FindNthDayOfMonth(DateTime monthToFind, int n, DayOfWeek dayOfWeek)
        {
            DateTime first = new DateTime(monthToFind.Year, monthToFind.Month, 1);
            while (first.DayOfWeek != dayOfWeek)
            {
                first = first.AddDays(1);
            }
            DateTime nth = first.AddDays(7 * (n - 1));
            return nth;
        }

        private static void Callback(object? state)
        {
            Reminder? r = state as Reminder;
            Console.WriteLine(r?.ReminderText);
                        
        }
    }
}
