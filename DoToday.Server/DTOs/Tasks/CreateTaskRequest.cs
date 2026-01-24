using System.ComponentModel.DataAnnotations;

namespace DoToday.Server.DTOs.Tasks;

public class CreateTaskRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;
}
