using DoToday.Server.Hubs;
using DoToday.Server.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DoToday.Server.Services.Implementations;

public class SyncNotificationService : ISyncNotificationService
{
    private readonly IHubContext<SyncHub> _hubContext;

    public SyncNotificationService(IHubContext<SyncHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyListCreatedAsync(int listId)
    {
        return _hubContext.Clients.All.SendAsync("ListCreated", listId);
    }

    public Task NotifyListUpdatedAsync(int listId)
    {
        return _hubContext.Clients.All.SendAsync("ListUpdated", listId);
    }

    public Task NotifyListDeletedAsync(int listId)
    {
        return _hubContext.Clients.All.SendAsync("ListDeleted", listId);
    }

    public Task NotifyTaskCreatedAsync(int listId, int taskId)
    {
        return _hubContext.Clients.Group($"list-{listId}").SendAsync("TaskCreated", listId, taskId);
    }

    public Task NotifyTaskUpdatedAsync(int listId, int taskId)
    {
        return _hubContext.Clients.Group($"list-{listId}").SendAsync("TaskUpdated", listId, taskId);
    }
}
