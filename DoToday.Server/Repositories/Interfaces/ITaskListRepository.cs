using DoToday.Server.Models;

namespace DoToday.Server.Repositories.Interfaces;

public interface ITaskListRepository
{
    Task<IEnumerable<TaskList>> GetAllAsync();
    Task<TaskList?> GetByIdAsync(int id);
    Task<TaskList> AddAsync(TaskList list);
    Task UpdateAsync(TaskList list);
    Task DeleteAsync(int id);
    Task<bool> ExistsWithNameAsync(string name, int? excludeId = null);
}
