using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<SQLiteReminderRepo> logger;

    public SQLiteReminderRepo(ILogger<SQLiteReminderRepo> logger)
    {
        this.logger = logger;
        _connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
        CreateTables();
        _ = DeleteOldRemindersAsync();
    }

    private void CreateTables()
    {
        logger.LogDebug("CreateTables");
        string sql = @"
CREATE TABLE IF NOT EXISTS Reminders (
id	INTEGER NOT NULL UNIQUE,
ReminderText	TEXT NOT NULL DEFAULT """",
ReminderTime	INTEGER NOT NULL,
Recurrence	INTEGER NOT NULL DEFAULT 0,
RecurrenceData	TEXT,
PRIMARY KEY(id AUTOINCREMENT)
)";

        try
        {
            var conn = new SqliteConnection(_connectionString);
            conn.Execute(sql);
        }
        catch (Exception e)
        {
            logger.LogError(e, "SQLite error executing - {sql}", sql);
            throw ;
        }
        
    }

    public async Task<Reminder> AddReminderAsync(Reminder item)
    {

        logger.LogDebug("AddReminderAsync");
        using SqliteConnection conn = new SqliteConnection(_connectionString);
        string sql = @"
INSERT INTO Reminders
(ReminderText,
ReminderTime, 
Recurrence,
RecurrenceData) 
VALUES (@ReminderText,
@ReminderTime,
@Recurrence,
@RecurrenceData);
SELECT last_insert_rowid();";            
        var newId = await conn.ExecuteScalarAsync<int>(sql, item);
        item.id = newId;
        return item;
    }

    public async Task<bool> DeleteReminderAsync(Reminder item)
    {
        logger.LogDebug("DeleteReminderAsync");
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
        logger.LogDebug("DeleteOldRemindersAsync");
        using SqliteConnection conn = new SqliteConnection(_connectionString);
        string sql = @"
DELETE FROM REMINDERS 
WHERE ReminderTime < datetime(""now"") and Recurrence = 0;
SELECT CHANGES();
";
        var numDeleted = await conn.ExecuteScalarAsync<int>(sql);
        return numDeleted > 0;
    }

    public async Task<List<Reminder>> GetRemindersAsync()
    {
        logger.LogDebug("GetRemindersAsync");
        using SqliteConnection conn = new SqliteConnection(_connectionString);
        var result = await conn.QueryAsync<Reminder>(@"select * from Reminders");
        return result.ToList();
    }
}
