using DoToday.Server.DTOs;
using DoToday.Server.DTOs.Lists;

namespace DoToday.Server.Services.Interfaces;

public interface ITaskListService
{
    Task<IEnumerable<TaskListSummaryDto>> GetAllListsAsync();
    Task<TaskListDto?> GetListByIdAsync(int id);
    Task<TaskListDto> CreateListAsync(CreateTaskListRequest request);
    Task<TaskListDto?> UpdateListAsync(int id, UpdateTaskListRequest request);
    Task<bool> DeleteListAsync(int id);
}
