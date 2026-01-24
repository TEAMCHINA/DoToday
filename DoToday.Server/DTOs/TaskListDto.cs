namespace DoToday.Server.DTOs;

public class TaskListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TaskDto> Tasks { get; set; } = new();
}
