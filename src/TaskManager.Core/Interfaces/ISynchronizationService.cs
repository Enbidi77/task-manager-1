using System;
using System.Threading.Tasks;

namespace TaskManager.Core.Interfaces;

public interface ISynchronizationService
{
    Task SyncAsync();
    Task<DateTimeOffset?> GetLastSyncTimeAsync();
    Task<bool> IsSyncingAsync();
}
