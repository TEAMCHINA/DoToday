using DoToday.Server.Controllers;
using DoToday.Server.DTOs;
using DoToday.Server.DTOs.Tasks;
using DoToday.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DoToday.Server.Tests;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _serviceMock;
    private readonly TasksController _sut;

    public TasksControllerTests()
    {
        _serviceMock = new Mock<ITaskService>();
        _sut = new TasksController(_serviceMock.Object);
    }

    [Fact]
    public async Task Create_WhenListExists_ReturnsCreatedWithTask()
    {
        // Arrange
        var listId = 1;
        var request = new CreateTaskRequest { Title = "New Task" };
        var taskDto = new TaskDto
        {
            Id = 42,
            Title = "New Task",
            IsCompleted = false,
            ListId = listId
        };

        _serviceMock
            .Setup(s => s.CreateTaskAsync(listId, request))
            .ReturnsAsync(taskDto);

        // Act
        var result = await _sut.Create(listId, request);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        var response = Assert.IsType<CreateTaskResponse>(createdResult.Value);
        Assert.Equal(taskDto.Id, response.Task!.Id);
        Assert.Equal(taskDto.Title, response.Task.Title);
    }

    [Fact]
    public async Task Create_WhenListDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var listId = 999;
        var request = new CreateTaskRequest { Title = "New Task" };

        _serviceMock
            .Setup(s => s.CreateTaskAsync(listId, request))
            .ReturnsAsync((TaskDto?)null);

        // Act
        var result = await _sut.Create(listId, request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.NotNull(notFoundResult.Value);
    }

    [Fact]
    public async Task Update_WhenTaskExists_ReturnsOkWithTask()
    {
        // Arrange
        var listId = 1;
        var taskId = 42;
        var request = new UpdateTaskRequest { IsCompleted = true };
        var taskDto = new TaskDto
        {
            Id = taskId,
            Title = "Test Task",
            IsCompleted = true,
            ListId = listId
        };

        _serviceMock
            .Setup(s => s.UpdateTaskStatusAsync(listId, taskId, request))
            .ReturnsAsync(taskDto);

        // Act
        var result = await _sut.Update(listId, taskId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<UpdateTaskResponse>(okResult.Value);
        Assert.Equal(taskDto.Id, response.Task!.Id);
        Assert.True(response.Task.IsCompleted);
    }

    [Fact]
    public async Task Update_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var listId = 1;
        var taskId = 999;
        var request = new UpdateTaskRequest { IsCompleted = true };

        _serviceMock
            .Setup(s => s.UpdateTaskStatusAsync(listId, taskId, request))
            .ReturnsAsync((TaskDto?)null);

        // Act
        var result = await _sut.Update(listId, taskId, request);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }
}
