namespace DoToday.Server.DTOs.Lists;

public class GetTaskListsResponse
{
    public IEnumerable<TaskListSummaryDto> Lists { get; set; } = new List<TaskListSummaryDto>();
}
