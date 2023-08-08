using Dapper;
using Microsoft.Data.Sqlite;
using Reminder_WPF.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Reminder_WPF.Services;

public class SQLiteReminderRepo : IDataRepo
{

    private string _connectionString;

    public SQLiteReminderRepo()
    {
        _connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
        CreateTables();
        DeleteOldRemindersAsync();
    }

    private void CreateTables()
    {
        string sql = @"
CREATE TABLE IF NOT EXISTS Reminders (
id	INTEGER NOT NULL UNIQUE,
ReminderText	TEXT NOT NULL DEFAULT """",
ReminderTime	INTEGER NOT NULL,
IsRecurring	INTEGER NOT NULL DEFAULT 0,
PRIMARY KEY(id AUTOINCREMENT)
)";
        
        var conn = new SqliteConnection(_connectionString);
        conn.Execute(sql);
        
    }

    public async Task<Reminder> AddReminderAsync(Reminder item)
    {        
            using SqliteConnection conn = new SqliteConnection(_connectionString);
            string sql = @"
INSERT INTO Reminders
(ReminderText,
ReminderTime, IsRecurring) 
VALUES (@ReminderText,
@ReminderTime,
@IsRecurring);
SELECT last_insert_rowid();";
            
        var newId = await conn.ExecuteScalarAsync<int>(sql, item);
        item.id = newId;
        return item;
    }

    public async Task<bool> DeleteReminderAsync(Reminder item)
    {
        using SqliteConnection conn = new SqliteConnection(_connectionString);
        string sql = @"
DELETE FROM REMINDERS 
WHERE ID = @ID;
SELECT CHANGES();
";
        var numDeleted = await conn.ExecuteScalarAsync<int>(sql, new {ID = item.id});
        return numDeleted > 0;
    }

    public async Task<bool> DeleteOldRemindersAsync()
    {
        using SqliteConnection conn = new SqliteConnection(_connectionString);
        string sql = @"
DELETE FROM REMINDERS 
WHERE ReminderTime < datetime(""now"") and IsRecurring = false;
SELECT CHANGES();
";
        var numDeleted = await conn.ExecuteScalarAsync<int>(sql);
        return numDeleted > 0;
    }





    public async Task<List<Reminder>> GetRemindersAsync()
    {
        using SqliteConnection conn = new SqliteConnection(_connectionString);
        var result = await conn.QueryAsync<Reminder>(@"select * from Reminders");
        return result.ToList();
    }
}
