using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder_WPF
{
    internal class TestReminderRepo : IDataRepo
    {
        public List<Reminder> GetReminders()
        {
            return new List<Reminder>()
            {
                new Reminder()
                {
                     ReminderText = "test reminder 1", ReminderTime = DateTime.Now.AddSeconds(10) 
                },
                new Reminder()
                {
                     ReminderText = "test reminder 2", ReminderTime = DateTime.Now.AddSeconds(20)
                },
                new Reminder()
                {
                     ReminderText = "test reminder 3", ReminderTime = DateTime.Now.AddSeconds(30)
                },
                new Reminder()
                {
                     ReminderText = "test reminder 4", ReminderTime = DateTime.Now.AddSeconds(40)
                },
            };
        }
    }
}
