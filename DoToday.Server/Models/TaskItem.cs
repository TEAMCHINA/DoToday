namespace DoToday.Server.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int ListId { get; set; }
    public TaskList List { get; set; } = null!;
}
