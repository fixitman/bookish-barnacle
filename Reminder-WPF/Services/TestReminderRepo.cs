using Reminder_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reminder_WPF.Services
{
    internal class TestReminderRepo : IDataRepo
    {
        private List<Reminder> list = new List<Reminder>()
            {
                new Reminder()
        {
            ReminderText = "test reminder 1", ReminderTime = DateTime.Now.AddDays(10), id = 1
                },
                new Reminder()
        {
            ReminderText = "test reminder 2", ReminderTime = DateTime.Now.AddHours(20), id = 2
                },
                new Reminder()
        {
            ReminderText = "test reminder 3", ReminderTime = DateTime.Now.AddMinutes(30), id = 3
                },
                new Reminder()
        {
            ReminderText = "test reminder 4", ReminderTime = DateTime.Now.AddSeconds(40), id = 4
                },
            };


        public List<Reminder> GetReminders()
        {
            return list.ToList();
        }
        
        public async Task<List<Reminder>> GetRemindersAsync()
        {
            return await Task.Run<List<Reminder>>(() =>
            {
                return GetReminders();
            });
        }


        public Reminder AddReminder(Reminder item)
        {
            if (list.Count == 0)
            {
                item.id = 1;
            }
            else
            {
                item.id = list.Max(r => r.id) + 1;
            }
            list.Add(item);
            return item;
        }

        public async Task<Reminder> AddReminderAsync(Reminder item)
        {
            return await Task.Run(() =>
            {
                return AddReminder(item);
            });
        }


        public async Task<bool> DeleteReminderAsync(Reminder item)
        {
            return await Task.Run(() =>
            {
                return DeleteReminder(item);
            });
        }

        public bool DeleteReminder(Reminder item)
        {
            list.Remove(item);
            return true;
        }
        
        

    }
}
