using DoToday.Server.DTOs.Lists;
using DoToday.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DoToday.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ListsController : ControllerBase
{
    private readonly ITaskListService _service;
    private readonly ISyncNotificationService _syncNotificationService;

    public ListsController(ITaskListService service, ISyncNotificationService syncNotificationService)
    {
        _service = service;
        _syncNotificationService = syncNotificationService;
    }

    [HttpGet(Name = "GetLists")]
    public async Task<ActionResult<GetTaskListsResponse>> GetAll()
    {
        var lists = await _service.GetAllListsAsync();
        return Ok(new GetTaskListsResponse { Lists = lists });
    }

    [HttpGet("{id}", Name = "GetListById")]
    public async Task<ActionResult<GetTaskListResponse>> GetById(int id)
    {
        var list = await _service.GetListByIdAsync(id);
        if (list == null) return NotFound();

        return Ok(new GetTaskListResponse { List = list });
    }

    [HttpPost(Name = "CreateList")]
    [ProducesResponseType(typeof(CreateTaskListResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CreateTaskListResponse>> Create(CreateTaskListRequest request)
    {
        try
        {
            var list = await _service.CreateListAsync(request);
            await _syncNotificationService.NotifyListCreatedAsync(list.Id);
            var response = new CreateTaskListResponse { List = list };
            return CreatedAtAction(nameof(GetById), new { id = list.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}", Name = "UpdateList")]
    public async Task<ActionResult<UpdateTaskListResponse>> Update(int id, UpdateTaskListRequest request)
    {
        try
        {
            var list = await _service.UpdateListAsync(id, request);
            if (list == null) return NotFound();

            await _syncNotificationService.NotifyListUpdatedAsync(id);
            return Ok(new UpdateTaskListResponse { List = list });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}", Name = "DeleteList")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteListAsync(id);
        if (!deleted) return NotFound();

        await _syncNotificationService.NotifyListDeletedAsync(id);
        return NoContent();
    }
}
