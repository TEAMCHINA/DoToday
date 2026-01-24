using DoToday.Server.DTOs;
using DoToday.Server.DTOs.Tasks;

namespace DoToday.Server.Services.Interfaces;

public interface ITaskService
{
    Task<TaskDto?> GetTaskAsync(int listId, int taskId);
    Task<TaskDto?> CreateTaskAsync(int listId, CreateTaskRequest request);
    Task<TaskDto?> UpdateTaskAsync(int listId, int taskId, UpdateTaskRequest request);
}
