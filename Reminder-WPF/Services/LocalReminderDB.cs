
using Microsoft.EntityFrameworkCore;
using Reminder_WPF.Models;
using System;
using System.Collections.Generic;


public class LocalReminderDB: DbContext
{
    public DbSet<Reminder> Reminders { get; set;}
    public String DbPath;


    public LocalReminderDB()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "LocalReminders.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }





}