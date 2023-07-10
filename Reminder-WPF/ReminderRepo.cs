using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder_WPF
{
    internal class ReminderRepo : IDataRepo
    {
        public List<Reminder> GetReminders()
        {
            return new List<Reminder>()
            {
                new Reminder()
                {
                     ReminderText = "test reminder1", ReminderTime = DateTime.Now.AddMinutes(1)
                },
                new Reminder()
                {
                     ReminderText = "test reminder2", ReminderTime = DateTime.Now.AddMinutes(2)
                },
                new Reminder()
                {
                     ReminderText = "test reminder3", ReminderTime = DateTime.Now.AddMinutes(3)
                },
                new Reminder()
                {
                     ReminderText = "test reminder4", ReminderTime = DateTime.Now.AddMinutes(4)
                },
            };
        }
    }
}
