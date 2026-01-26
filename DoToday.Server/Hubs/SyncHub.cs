using Microsoft.AspNetCore.SignalR;

namespace DoToday.Server.Hubs;

public class SyncHub : Hub
{
    public Task JoinListGroup(int listId) => Groups.AddToGroupAsync(Context.ConnectionId, $"list-{listId}");
    public Task LeaveListGroup(int listId) => Groups.RemoveFromGroupAsync(Context.ConnectionId, $"list-{listId}");
}
