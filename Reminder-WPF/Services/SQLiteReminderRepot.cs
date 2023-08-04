using Reminder_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reminder_WPF.Services;

public class SQLiteReminderRepot : IDataRepo
{
    public SQLiteReminderRepot()
    {

    }    

    public Task<Reminder> AddReminderAsync(Reminder item)
    {
        throw new NotImplementedException();
    }    

    public Task<bool> DeleteReminderAsync(Reminder item)
    {
        throw new NotImplementedException();
    }    

    public Task<List<Reminder>> GetRemindersAsync()
    {
        throw new NotImplementedException();
    }
}
