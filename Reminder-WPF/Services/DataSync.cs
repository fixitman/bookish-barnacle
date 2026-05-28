using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reminder_WPF.Models;

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
            _cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CacheFileName);
            EnsureCacheFile();
        }

        // Represents a pending change to be applied to remote when online
        private class PendingChange
        {
            public required SyncOperation Operation { get; set; } // "create", "update", "delete"
            public required Reminder Item { get; set; }
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
            _isSyncing = true;
            if (_remote == null || _local == null) return; // sanity check - should not happen
            if (!await _remote.IsAvailableAsync()) return; // remote unavailable - likely offline
            // First, attempt to push cached changes
            var cached = LoadCache();
            if (cached.Any())
            {
                var remaining = new List<PendingChange>();
                foreach (var change in cached)
                {
                    try
                    {
                        switch (change.Operation)
                        {
                            case SyncOperation.Create:
                                await _remote.AddReminderAsync(change.Item);
                                break;
                            case SyncOperation.Update:
                                await _remote.UpdateReminderAsync(change.Item);
                                break;
                            case SyncOperation.Delete:
                                await _remote.DeleteReminderAsync(change.Item);
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
            try
            {
                var remoteResult = await _remote.GetRemindersAsync();
                if (remoteResult.IsFailure) return; // remote failure - can't do much
                remoteList = remoteResult.Value ?? new List<Reminder>();
            }
            catch
            {
                return; // offline or remote unavailable
            }

            var result = await _local.GetRemindersAsync();
            if (result.IsFailure || result.Value == null) return; // local store failure - can't do much
            var localById = result.Value.ToDictionary(r => r.id);

            // Upsert remote items into local
            foreach (var r in remoteList)
            {
                if (r == null) continue;
                if (localById.TryGetValue(r.id, out var existing))
                {
                    
                    // if remote is newer, update local. 
                    if (existing.LastUpdated < r.LastUpdated){
                        await _local.UpdateReminderAsync(r);     
                    // if local is newer, update remote.                    
                    }else if(existing.LastUpdated > r.LastUpdated){
                        await _remote.UpdateReminderAsync(existing);                        
                    }
                    //else do nothing - they are the same
                }
                else
                {
                    await _local.AddReminderAsync(r);
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
