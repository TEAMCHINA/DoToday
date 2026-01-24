using DoToday.Server.Data;
using DoToday.Server.Models;
using DoToday.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoToday.Server.Repositories.Implementations;

public class TaskListRepository : ITaskListRepository
{
    private readonly AppDbContext _context;

    public TaskListRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskList>> GetAllAsync()
    {
        return await _context.TaskLists
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TaskList?> GetByIdAsync(int id)
    {
        return await _context.TaskLists
            .Include(l => l.Tasks)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<TaskList> AddAsync(TaskList list)
    {
        _context.TaskLists.Add(list);
        await _context.SaveChangesAsync();
        return list;
    }

    public async Task UpdateAsync(TaskList list)
    {
        _context.TaskLists.Update(list);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var list = await _context.TaskLists.FindAsync(id);
        if (list != null)
        {
            _context.TaskLists.Remove(list);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsWithNameAsync(string name, int? excludeId = null)
    {
        return await _context.TaskLists
            .AnyAsync(l => l.Name == name && (excludeId == null || l.Id != excludeId));
    }
}
