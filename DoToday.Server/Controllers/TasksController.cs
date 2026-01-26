using DoToday.Server.DTOs.Tasks;
using DoToday.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DoToday.Server.Controllers;

[ApiController]
[Route("api/lists/{listId}/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;
    private readonly ISyncNotificationService _syncNotificationService;

    public TasksController(ITaskService service, ISyncNotificationService syncNotificationService)
    {
        _service = service;
        _syncNotificationService = syncNotificationService;
    }

    [HttpPost(Name = "CreateTask")]
    [ProducesResponseType(typeof(CreateTaskResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CreateTaskResponse>> Create(int listId, CreateTaskRequest request)
    {
        var task = await _service.CreateTaskAsync(listId, request);
        if (task == null) return NotFound(new { message = "List not found" });

        await _syncNotificationService.NotifyTaskCreatedAsync(listId, task.Id);
        var response = new CreateTaskResponse { Task = task };
        return Created(string.Empty, response);
    }

    [HttpPut("{taskId}", Name = "UpdateTask")]
    public async Task<ActionResult<UpdateTaskResponse>> Update(int listId, int taskId, UpdateTaskRequest request)
    {
        var task = await _service.UpdateTaskStatusAsync(listId, taskId, request);
        if (task == null) return NotFound();

        await _syncNotificationService.NotifyTaskUpdatedAsync(listId, taskId);
        return Ok(new UpdateTaskResponse { Task = task });
    }
}
