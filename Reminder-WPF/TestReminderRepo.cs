using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder_WPF
{
    internal class TestReminderRepo : IDataRepo
    {
        private List<Reminder> list = new List<Reminder>()
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

        

        public Reminder AddReminder(Reminder item)
        {
            if (list.Count == 0)
            {
                item.id = 1;
            }
            else
            {
            var max = list.OrderBy(x => x.id).Last().id;
            item.id = max + 1;
            }
            list.Add(item);
            return (item);
        }

        public List<Reminder> GetReminders()
        {
            return list.ToList();
        }

        bool IDataRepo.DeleteReminder(Reminder item)
        {
            list.Remove(item);
            return true;
        }
    }
}
