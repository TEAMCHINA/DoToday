using DoToday.Server.DTOs.Lists;
using DoToday.Server.Models;
using DoToday.Server.Repositories.Interfaces;
using DoToday.Server.Services.Implementations;
using Moq;

namespace DoToday.Server.Tests;

public class TaskListServiceTests
{
    private readonly Mock<ITaskListRepository> _repositoryMock;
    private readonly TaskListService _sut;

    public TaskListServiceTests()
    {
        _repositoryMock = new Mock<ITaskListRepository>();
        _sut = new TaskListService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllListsAsync_ReturnsAllLists()
    {
        // Arrange
        var lists = new List<TaskList>
        {
            new() { Id = 1, Name = "List 1" },
            new() { Id = 2, Name = "List 2" }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(lists);

        // Act
        var result = await _sut.GetAllListsAsync();

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("List 1", resultList[0].Name);
        Assert.Equal("List 2", resultList[1].Name);
    }

    [Fact]
    public async Task GetAllListsAsync_WhenEmpty_ReturnsEmptyCollection()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<TaskList>());

        // Act
        var result = await _sut.GetAllListsAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetListByIdAsync_WhenExists_ReturnsListWithTasks()
    {
        // Arrange
        var list = new TaskList
        {
            Id = 1,
            Name = "Test List",
            Tasks = new List<TaskItem>
            {
                new() { Id = 1, Title = "Task 1", IsCompleted = false, ListId = 1 },
                new() { Id = 2, Title = "Task 2", IsCompleted = true, ListId = 1 }
            }
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(list);

        // Act
        var result = await _sut.GetListByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test List", result.Name);
        Assert.Equal(2, result.Tasks.Count);
        Assert.Equal("Task 1", result.Tasks[0].Title);
    }

    [Fact]
    public async Task GetListByIdAsync_WhenNotExists_ReturnsNull()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((TaskList?)null);

        // Act
        var result = await _sut.GetListByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateListAsync_WhenNameIsUnique_CreatesAndReturnsList()
    {
        // Arrange
        var request = new CreateTaskListRequest { Name = "New List" };

        _repositoryMock
            .Setup(r => r.ExistsWithNameAsync("New List", null))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskList>()))
            .ReturnsAsync((TaskList l) => { l.Id = 1; return l; });

        // Act
        var result = await _sut.CreateListAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New List", result.Name);
        _repositoryMock.Verify(r => r.AddAsync(It.Is<TaskList>(l => l.Name == "New List")), Times.Once);
    }

    [Fact]
    public async Task CreateListAsync_WhenNameExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new CreateTaskListRequest { Name = "Existing List" };

        _repositoryMock
            .Setup(r => r.ExistsWithNameAsync("Existing List", null))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.CreateListAsync(request));

        Assert.Contains("Existing List", exception.Message);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskList>()), Times.Never);
    }

    [Fact]
    public async Task UpdateListAsync_WhenExistsAndNameIsUnique_UpdatesAndReturnsList()
    {
        // Arrange
        var existingList = new TaskList { Id = 1, Name = "Old Name", Tasks = new List<TaskItem>() };
        var request = new UpdateTaskListRequest { Name = "New Name" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingList);

        _repositoryMock
            .Setup(r => r.ExistsWithNameAsync("New Name", 1))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.UpdateListAsync(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Name", result.Name);
        _repositoryMock.Verify(r => r.UpdateAsync(existingList), Times.Once);
    }

    [Fact]
    public async Task UpdateListAsync_WhenNotExists_ReturnsNull()
    {
        // Arrange
        var request = new UpdateTaskListRequest { Name = "New Name" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((TaskList?)null);

        // Act
        var result = await _sut.UpdateListAsync(999, request);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskList>()), Times.Never);
    }

    [Fact]
    public async Task UpdateListAsync_WhenNameExistsOnDifferentList_ThrowsInvalidOperationException()
    {
        // Arrange
        var existingList = new TaskList { Id = 1, Name = "List 1", Tasks = new List<TaskItem>() };
        var request = new UpdateTaskListRequest { Name = "List 2" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingList);

        _repositoryMock
            .Setup(r => r.ExistsWithNameAsync("List 2", 1))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.UpdateListAsync(1, request));

        Assert.Contains("List 2", exception.Message);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskList>()), Times.Never);
    }

    [Fact]
    public async Task DeleteListAsync_WhenExists_DeletesAndReturnsTrue()
    {
        // Arrange
        var existingList = new TaskList { Id = 1, Name = "Test List" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingList);

        // Act
        var result = await _sut.DeleteListAsync(1);

        // Assert
        Assert.True(result);
        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteListAsync_WhenNotExists_ReturnsFalse()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((TaskList?)null);

        // Act
        var result = await _sut.DeleteListAsync(999);

        // Assert
        Assert.False(result);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}
