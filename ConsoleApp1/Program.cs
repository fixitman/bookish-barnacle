using System.Data;
using System.Net.Http.Json;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ReminderScheduler scheduler = new ReminderScheduler();
            Reminder r = new Reminder { 
                id = 1, 
                ReminderTime = DateTime.Now, 
                ReminderText = "You have been reminded!", 
                Recurrence = Reminder.RecurrenceType.Weekly,
                RecurrenceData = "fri"
            };

            scheduler.ScheduleReminder(r, Callback);
            //Thread.Sleep(1000);
            //r.ReminderText += " Changed!";
            //r.ReminderTime = DateTime.Now.AddSeconds(9);
            //scheduler.UpdateReminder(r);
           
            Console.ReadLine();   //to keep the program running
        }

        private static void Callback(object? state)
        {
            Reminder? r = state as Reminder;
            Console.WriteLine(r?.ReminderText);
        }
    }
}
