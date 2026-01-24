using DoToday.Server.DTOs;
using DoToday.Server.DTOs.Lists;
using DoToday.Server.Models;
using DoToday.Server.Repositories.Interfaces;
using DoToday.Server.Services.Interfaces;

namespace DoToday.Server.Services.Implementations;

public class TaskListService : ITaskListService
{
    private readonly ITaskListRepository _repository;

    public TaskListService(ITaskListRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TaskListSummaryDto>> GetAllListsAsync()
    {
        var lists = await _repository.GetAllAsync();
        return lists.Select(l => new TaskListSummaryDto
        {
            Id = l.Id,
            Name = l.Name
        });
    }

    public async Task<TaskListDto?> GetListByIdAsync(int id)
    {
        var list = await _repository.GetByIdAsync(id);
        if (list == null) return null;

        return MapToDto(list);
    }

    public async Task<TaskListDto> CreateListAsync(CreateTaskListRequest request)
    {
        if (await _repository.ExistsWithNameAsync(request.Name))
        {
            throw new InvalidOperationException($"A list with name '{request.Name}' already exists.");
        }

        var list = new TaskList { Name = request.Name };
        await _repository.AddAsync(list);

        return MapToDto(list);
    }

    public async Task<TaskListDto?> UpdateListAsync(int id, UpdateTaskListRequest request)
    {
        var list = await _repository.GetByIdAsync(id);
        if (list == null) return null;

        if (await _repository.ExistsWithNameAsync(request.Name, id))
        {
            throw new InvalidOperationException($"A list with name '{request.Name}' already exists.");
        }

        list.Name = request.Name;
        await _repository.UpdateAsync(list);

        return MapToDto(list);
    }

    // Only supporting actual deletion for now; there may be a use case for soft delete/hiding lists,
    // but we'll build that when we need it.
    public async Task<bool> DeleteListAsync(int id)
    {
        var list = await _repository.GetByIdAsync(id);
        if (list == null) return false;

        await _repository.DeleteAsync(id);
        return true;
    }

    private static TaskListDto MapToDto(TaskList list)
    {
        return new TaskListDto
        {
            Id = list.Id,
            Name = list.Name,
            Tasks = list.Tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                IsCompleted = t.IsCompleted,
                ListId = t.ListId
            }).ToList()
        };
    }
}
