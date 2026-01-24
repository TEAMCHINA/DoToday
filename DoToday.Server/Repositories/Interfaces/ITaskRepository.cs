using DoToday.Server.Models;

namespace DoToday.Server.Repositories.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(int listId, int taskId);
    Task<TaskItem> AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(int listId, int taskId);
}
