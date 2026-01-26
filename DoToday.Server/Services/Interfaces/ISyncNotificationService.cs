namespace DoToday.Server.Services.Interfaces;

public interface ISyncNotificationService
{
    Task NotifyListCreatedAsync(int listId);
    Task NotifyListUpdatedAsync(int listId);
    Task NotifyListDeletedAsync(int listId);
    Task NotifyTaskCreatedAsync(int listId, int taskId);
    Task NotifyTaskUpdatedAsync(int listId, int taskId);
}
