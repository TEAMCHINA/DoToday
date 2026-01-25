using DoToday.Server.Data;
using DoToday.Server.Models;
using DoToday.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoToday.Server.Repositories.Implementations;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(int listId, int taskId)
    {
        // Task IDs should be unique while we're using SQL/SQLite, but if we move to a document store or
        // something where task IDs are only unique in the context of their parent list, then we'll need
        // both IDs, but this also protects us against data anomalies where a change is requested from a
        // list that doesn't contain the task.
        return await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == taskId && t.ListId == listId);
    }

    public async Task<TaskItem> AddAsync(TaskItem task)
    {
        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _context.TaskItems.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int listId, int taskId)
    {
        var task = await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == taskId && t.ListId == listId);
        if (task != null)
        {
            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}
