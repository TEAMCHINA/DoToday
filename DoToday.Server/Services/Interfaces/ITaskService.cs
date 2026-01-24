using DoToday.Server.DTOs;
using DoToday.Server.DTOs.Tasks;

namespace DoToday.Server.Services.Interfaces;

public interface ITaskService
{
    Task<TaskDto?> CreateTaskAsync(int listId, CreateTaskRequest request);
    Task<TaskDto?> UpdateTaskStatusAsync(int listId, int taskId, UpdateTaskRequest request);
}
