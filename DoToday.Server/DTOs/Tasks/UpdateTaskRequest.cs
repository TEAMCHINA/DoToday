namespace DoToday.Server.DTOs.Tasks;

public class UpdateTaskRequest
{
    public string? Title { get; set; }
    public bool? IsCompleted { get; set; }
}
