using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reminder_WPF.Models;
using Reminder_WPF.Utilities;

namespace Reminder_WPF.Services
{
    public enum SyncOperation
        {
            Create, Update, Delete
        }

    public interface IDataSync
    {
        Task QueueChangeAsync(Reminder reminder, SyncOperation operation);
        Task SyncAsync();
    }

    // Minimal synchronizer: pushes local changes to remote (caching when offline)
    // and pulls remote changes into local store.
    public class DataSync : IDataSync
    {
        private readonly IDataRepo _local;
        private readonly IDataRepo _remote;
        private readonly string _cachePath;
        private readonly object _cacheLock = new object();        
        private static bool _isSyncing = false; 
        private const string CacheFileName = "reminder_sync_cache.json";

        public DataSync(IDataRepo localRepo, IDataRepo remoteRepo, ILogger logger)
        {
            _local = localRepo ?? throw new ArgumentNullException(nameof(localRepo));
            _remote = remoteRepo ?? throw new ArgumentNullException(nameof(remoteRepo));
            _cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"FixitmanMike" ,CacheFileName);
            EnsureCacheFile();
        }

        // Represents a pending change to be applied to remote when online
        private class PendingChange
        {
            private static TimeSpan MAX_AGE = TimeSpan.FromDays(1);
            public required SyncOperation Operation { get; set; } // "create", "update", "delete"
            public required Reminder Item { get; set; }
            public DateTime ExpiresAt  = DateTime.UtcNow + MAX_AGE; 
        }

        private void EnsureCacheFile()
        {
            lock (_cacheLock)
            {
                if (!File.Exists(_cachePath)) File.WriteAllText(_cachePath, "[]");
            }
        }

        private List<PendingChange> LoadCache()
        {
            lock (_cacheLock)
            {
                var json = File.ReadAllText(_cachePath);
                if (string.IsNullOrWhiteSpace(json)) return new List<PendingChange>();
                return JsonSerializer.Deserialize<List<PendingChange>>(json) ?? new List<PendingChange>();
            }
        }

        private void SaveCache(List<PendingChange> list)
        {
            lock (_cacheLock)
            {
                var json = JsonSerializer.Serialize(list);
                File.WriteAllText(_cachePath, json);
            }
        }

        // Queue an operation to be sent to remote. This happens automatically when local changes are made.
        public Task QueueChangeAsync(Reminder reminder, SyncOperation operation)
        {
            if (reminder == null) throw new ArgumentNullException(nameof(reminder));

            var list = LoadCache();
            list.Add(new PendingChange { Operation = operation, Item = reminder });
            SaveCache(list);
            return Task.CompletedTask;
        }

        // Try to flush cache to remote and then pull remote state into local
        public async Task SyncAsync()
        {
            if (_isSyncing) return; // simple guard to prevent concurrent syncs
            if (_remote == null || _local == null) return; // sanity check - should not happen
            if (!await _remote.IsAvailableAsync()) return; // remote unavailable - likely offline
            _isSyncing = true;
            // First, attempt to push cached changes
            var cached = LoadCache();
            if (cached.Any())
            {
                var remaining = new List<PendingChange>();
                foreach (var change in cached)
                {
                    if (change.ExpiresAt < DateTime.UtcNow) continue; // skip stale changes
                    try
                    {
                        switch (change.Operation)
                        {
                            case SyncOperation.Create:
                                var AddResult = await _remote.AddReminderAsync(change.Item);
                                if (AddResult.IsFailure) throw new InvalidOperationException("Failed to add reminder to remote source.");
                                break;
                            case SyncOperation.Update:
                                var UpdateResult = await _remote.UpdateReminderAsync(change.Item);
                                if (UpdateResult.IsFailure) throw new InvalidOperationException("Failed to update reminder on remote source.");
                                break;
                            case SyncOperation.Delete:
                                var DeleteResult = await _remote.DeleteReminderAsync(change.Item);
                                if (DeleteResult.IsFailure) throw new InvalidOperationException("Failed to delete reminder from remote source.");
                                break;
                            default:
                                // unknown operation - ignore
                                break;
                        }
                    }
                    catch
                    {
                        // failed to push this change (likely offline) - keep it
                        remaining.Add(change);
                    }
                }

                SaveCache(remaining);
            }

            // Pull remote list and merge into local store. If remote call fails, bail out (offline).
            List<Reminder> remoteList;
            Dictionary<string, Reminder> localById;
            Result<List<Reminder>> result;
            try
            {
                var remoteResult = await _remote.GetRemindersAsync();
                if (remoteResult.IsFailure) throw new InvalidOperationException("Failed to retrieve reminders from remote source."); // remote failure - can't do much
                remoteList = remoteResult.Value ?? new List<Reminder>();
                result = await _local.GetRemindersAsync();
                if (result.IsFailure || result.Value == null) throw new InvalidOperationException("Failed to retrieve reminders from local source."); // local store failure - can't do much
                localById = result.Value.ToDictionary(r => r.id);
            }
            catch
            {
                _isSyncing = false;
                return; // offline or remote unavailable
            }


            // Upsert remote items into local
            foreach (var r in remoteList)
            {
                try
                {
                    if (r == null) continue;
                    if (localById.TryGetValue(r.id, out var existing))
                    {
                        
                        // if remote is newer, update local. 
                        if (existing.LastUpdated < r.LastUpdated){
                        var UpdateResult = await _local.UpdateReminderAsync(r);     
                        // if local is newer, update remote.                    
                        }else if(existing.LastUpdated > r.LastUpdated){
                            var UpdateResult = await _remote.UpdateReminderAsync(existing);
                            if (UpdateResult.IsFailure) throw new InvalidOperationException("Failed to update reminder on remote source.");
                        }
                        //else do nothing - they are the same
                    }
                    else
                    {
                        await _local.AddReminderAsync(r);
                    }
                }
                catch (System.Exception)
                {
                    _isSyncing = false;
                    return; // remote failure - likely offline - bail out and try again later
                }
            }
            foreach (var local in result.Value)
            {
                if (local == null) continue;
                if (!remoteList.Any(r => r.id == local.id))
                {
                    await _local.DeleteReminderAsync(local);
                }
            }

            _isSyncing = false;
        }
    }
}
