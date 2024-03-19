using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Reminder_WPF.Models;
using Reminder_WPF.Utilities;
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

    public async Task<Result<Reminder>> AddReminderAsync(Reminder item)
    {
        logger.LogDebug("AddReminderAsync");
        try
        {
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
            return Result.Ok(item);
        }
        catch (Exception e)
        {
            logger.LogError($"There was a problem. {e.Message}");
            return Result.Fail<Reminder>(e.Message);
        }
    }

    public async Task<Result> DeleteReminderAsync(Reminder item)
    {
        logger.LogDebug("DeleteReminderAsync");
        try
        {
            using SqliteConnection conn = new SqliteConnection(_connectionString);
            string sql = @"
DELETE FROM REMINDERS 
WHERE ID = @ID;
SELECT CHANGES();
";
           await conn.ExecuteScalarAsync<int>(sql, new { ID = item.id });
           return Result.Ok();
        }
        catch (Exception e )
        {
            logger.LogError($"There was a problem. {e.Message}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result> DeleteOldRemindersAsync()
    {
        logger.LogDebug("DeleteOldRemindersAsync");
        try
        {
            using SqliteConnection conn = new SqliteConnection(_connectionString);
            string sql = @"
DELETE FROM REMINDERS 
WHERE ReminderTime < datetime(""now"") and Recurrence = 0;
SELECT CHANGES();
";
            await conn.ExecuteScalarAsync<int>(sql);
            return Result.Ok();
        }
        catch (Exception e)
        {
            logger.LogError($"There was a problem. {e.Message}");
            return Result.Fail(e.Message);
        }
    }

    public async Task<Result<List<Reminder>>> GetRemindersAsync()
    {
        logger.LogDebug("GetRemindersAsync");
        try
        {
            using SqliteConnection conn = new SqliteConnection(_connectionString);
            var reminders = await conn.QueryAsync<Reminder>(@"select * from Reminders");
            return Result.Ok(reminders.ToList());
        }
        catch (Exception e)
        {
            logger.LogError($"There was a problem. {e.Message}");
            return Result.Fail<List<Reminder>>(e.Message);
        }
    }

    public async Task<Result<Reminder?>> GetReminderByIdAsync(int id)
    {
        logger.LogDebug($"GetReminderByIdAsync ({id})");
        try
        {
            using SqliteConnection conn = new SqliteConnection(_connectionString);
            Reminder? reminder = await conn.QueryFirstOrDefaultAsync<Reminder?>(@"select * from Reminders where id = @id", new { id = id });
            return Result.Ok(reminder);
        }
        catch (Exception e)
        {
            logger.LogError($"There was a problem. {e.Message}");
            return Result.Fail<Reminder?>(e.Message);
        }
    }
}
