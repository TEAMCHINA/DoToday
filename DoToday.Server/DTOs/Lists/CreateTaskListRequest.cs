using System.ComponentModel.DataAnnotations;

namespace DoToday.Server.DTOs.Lists;

public class CreateTaskListRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}
