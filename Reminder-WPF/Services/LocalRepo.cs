using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        item.LastUpdated = DateTime.Now.Ticks;
        db.Add(item);
        var r = await db.SaveChangesAsync();
        return Result.Ok<Reminder?>(null);
    }

    public async Task<Result> DeleteReminderAsync(Reminder item)
    {
        db.Remove(item);
        var r = await db.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<Reminder?>> GetReminderByIdAsync(int id)
    {
        var r = await db.Reminders.FirstOrDefaultAsync(item => item.id == id);
        return Result.Ok(r);
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

    public async Task<Result<Reminder?>> UpdateReminderAsync(Reminder r)
    {
        var reminder = await db.Reminders.FirstOrDefaultAsync(item => item.id == r.id);
        if (reminder == null)
        {
            return Result.Ok<Reminder?>(null);
        }
        reminder.LastUpdated = DateTime.Now.Ticks;
        reminder.Recurrence = r.Recurrence;
        reminder.RecurrenceData = r.RecurrenceData;
        reminder.ReminderText = r.ReminderText;
        reminder.ReminderTime = r.ReminderTime; 
        return Result.Ok<Reminder?>(reminder);
    }
}