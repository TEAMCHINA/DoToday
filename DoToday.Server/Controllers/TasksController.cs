using DoToday.Server.DTOs.Tasks;
using DoToday.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DoToday.Server.Controllers;

[ApiController]
[Route("api/lists/{listId}/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<CreateTaskResponse>> Create(int listId, CreateTaskRequest request)
    {
        var task = await _service.CreateTaskAsync(listId, request);
        if (task == null) return NotFound(new { message = "List not found" });

        var response = new CreateTaskResponse { Task = task };
        return CreatedAtAction(nameof(GetById), new { listId, taskId = task.Id }, response);
    }

    [HttpGet("{taskId}")]
    public async Task<ActionResult<GetTaskResponse>> GetById(int listId, int taskId)
    {
        var task = await _service.GetTaskAsync(listId, taskId);
        if (task == null) return NotFound();

        return Ok(new GetTaskResponse { Task = task });
    }

    [HttpPut("{taskId}")]
    public async Task<ActionResult<UpdateTaskResponse>> Update(int listId, int taskId, UpdateTaskRequest request)
    {
        var task = await _service.UpdateTaskAsync(listId, taskId, request);
        if (task == null) return NotFound();

        return Ok(new UpdateTaskResponse { Task = task });
    }
}
