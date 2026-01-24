using DoToday.Server.DTOs.Tasks;
using DoToday.Server.Models;
using DoToday.Server.Repositories.Interfaces;
using DoToday.Server.Services.Implementations;
using Moq;

namespace DoToday.Server.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<ITaskListRepository> _listRepositoryMock;
    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _listRepositoryMock = new Mock<ITaskListRepository>();
        _sut = new TaskService(_taskRepositoryMock.Object, _listRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateTaskAsync_WhenListExists_ReturnsTask()
    {
        // Arrange
        var listId = 1;
        var request = new CreateTaskRequest { Title = "Test Task" };
        var list = new TaskList { Id = listId, Name = "Test List" };

        _listRepositoryMock
            .Setup(r => r.GetByIdAsync(listId))
            .ReturnsAsync(list);

        _taskRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem t) => { t.Id = 42; return t; });

        // Act
        var result = await _sut.CreateTaskAsync(listId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
        Assert.Equal(listId, result.ListId);
        Assert.False(result.IsCompleted);
    }

    [Fact]
    public async Task CreateTaskAsync_WhenListDoesNotExist_ReturnsNull()
    {
        // Arrange
        var listId = 999;
        var request = new CreateTaskRequest { Title = "Test Task" };

        _listRepositoryMock
            .Setup(r => r.GetByIdAsync(listId))
            .ReturnsAsync((TaskList?)null);

        // Act
        var result = await _sut.CreateTaskAsync(listId, request);

        // Assert
        Assert.Null(result);
        _taskRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTaskStatusAsync_WhenTaskExists_UpdatesAndReturnsTask()
    {
        // Arrange
        var listId = 1;
        var taskId = 42;
        var request = new UpdateTaskRequest { IsCompleted = true };
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            IsCompleted = false,
            ListId = listId
        };

        _taskRepositoryMock
            .Setup(r => r.GetByIdAsync(listId, taskId))
            .ReturnsAsync(existingTask);

        // Act
        var result = await _sut.UpdateTaskStatusAsync(listId, taskId, request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsCompleted);
        _taskRepositoryMock.Verify(r => r.UpdateAsync(existingTask), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskStatusAsync_WhenTaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var listId = 1;
        var taskId = 999;
        var request = new UpdateTaskRequest { IsCompleted = true };

        _taskRepositoryMock
            .Setup(r => r.GetByIdAsync(listId, taskId))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _sut.UpdateTaskStatusAsync(listId, taskId, request);

        // Assert
        Assert.Null(result);
        _taskRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTaskStatusAsync_WhenIsCompletedIsNull_DoesNotChangeStatus()
    {
        // Arrange
        var listId = 1;
        var taskId = 42;
        var request = new UpdateTaskRequest { IsCompleted = null };
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            IsCompleted = false,
            ListId = listId
        };

        _taskRepositoryMock
            .Setup(r => r.GetByIdAsync(listId, taskId))
            .ReturnsAsync(existingTask);

        // Act
        var result = await _sut.UpdateTaskStatusAsync(listId, taskId, request);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsCompleted);
    }

    [Fact]
    public async Task UpdateTaskStatusAsync_CanToggleFromCompletedToIncomplete()
    {
        // Arrange
        var listId = 1;
        var taskId = 42;
        var request = new UpdateTaskRequest { IsCompleted = false };
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            IsCompleted = true,
            ListId = listId
        };

        _taskRepositoryMock
            .Setup(r => r.GetByIdAsync(listId, taskId))
            .ReturnsAsync(existingTask);

        // Act
        var result = await _sut.UpdateTaskStatusAsync(listId, taskId, request);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsCompleted);
    }
}
