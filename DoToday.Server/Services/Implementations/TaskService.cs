using DoToday.Server.DTOs;
using DoToday.Server.DTOs.Tasks;
using DoToday.Server.Models;
using DoToday.Server.Repositories.Interfaces;
using DoToday.Server.Services.Interfaces;

namespace DoToday.Server.Services.Implementations;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskListRepository _listRepository;

    public TaskService(ITaskRepository taskRepository, ITaskListRepository listRepository)
    {
        _taskRepository = taskRepository;
        _listRepository = listRepository;
    }

    public async Task<TaskDto?> GetTaskAsync(int listId, int taskId)
    {
        var task = await _taskRepository.GetByIdAsync(listId, taskId);
        if (task == null) return null;

        return MapToDto(task);
    }

    public async Task<TaskDto?> CreateTaskAsync(int listId, CreateTaskRequest request)
    {
        var list = await _listRepository.GetByIdAsync(listId);
        if (list == null) return null;

        var task = new TaskItem
        {
            Title = request.Title,
            IsCompleted = false,
            ListId = listId
        };

        await _taskRepository.AddAsync(task);
        return MapToDto(task);
    }

    public async Task<TaskDto?> UpdateTaskAsync(int listId, int taskId, UpdateTaskRequest request)
    {
        var task = await _taskRepository.GetByIdAsync(listId, taskId);
        if (task == null) return null;

        if (request.Title != null)
        {
            task.Title = request.Title;
        }

        if (request.IsCompleted.HasValue)
        {
            task.IsCompleted = request.IsCompleted.Value;
        }

        await _taskRepository.UpdateAsync(task);
        return MapToDto(task);
    }

    private static TaskDto MapToDto(TaskItem task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            IsCompleted = task.IsCompleted,
            ListId = task.ListId
        };
    }
}
