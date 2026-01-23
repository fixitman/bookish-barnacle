




using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Reminder_WPF.Models;
using Reminder_WPF.Services;
using Reminder_WPF.Utilities;




public class LocalRepo : IDataRepo
{
    
    private LocalReminderDB db;

    public LocalRepo()
    {
        db = new LocalReminderDB();
    }

    public async Task<Result<Reminder?>> AddReminderAsync(Reminder item)
    {
        db.Add(item);
        var r = await db.SaveChangesAsync();
        return Result.Ok();

    }

    public Task<Result> DeleteReminderAsync(Reminder item)
    {
        throw new System.NotImplementedException();
    }

    public Task<Result<Reminder?>> GetReminderByIdAsync(int id)
    {
        throw new System.NotImplementedException();
    }

    public async Task<Result<List<Reminder>>> GetRemindersAsync()
    {
        var item = new Reminder{ReminderText="hello",ReminderTime=DateTime.Now.AddHours(6.0),Recurrence=0, RecurrenceData="",LastUpdated=DateTime.Now.AddDays(-7.0).Ticks};
        var r =  db.AddAsync (item);
        await db.SaveChangesAsync();
        var reminders = await db.Reminders.ToListAsync();
        if(reminders != null)
        {
            return Result.Ok(reminders);
        }
        else
        {
            return Result.Fail<List<Reminder>>("GetRemindersAsync failed");
        }
        
    }

    public Task<Result<Reminder?>> UpdateReminderAsync(Reminder r)
    {
        throw new System.NotImplementedException();
    }
}