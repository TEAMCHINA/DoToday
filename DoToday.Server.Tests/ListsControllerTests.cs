using DoToday.Server.Controllers;
using DoToday.Server.DTOs;
using DoToday.Server.DTOs.Lists;
using DoToday.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DoToday.Server.Tests;

public class ListsControllerTests
{
    private readonly Mock<ITaskListService> _serviceMock;
    private readonly ListsController _sut;

    public ListsControllerTests()
    {
        _serviceMock = new Mock<ITaskListService>();
        _sut = new ListsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithLists()
    {
        // Arrange
        var lists = new List<TaskListSummaryDto>
        {
            new() { Id = 1, Name = "List 1" },
            new() { Id = 2, Name = "List 2" }
        };

        _serviceMock
            .Setup(s => s.GetAllListsAsync())
            .ReturnsAsync(lists);

        // Act
        var result = await _sut.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<GetTaskListsResponse>(okResult.Value);
        Assert.Equal(2, response.Lists!.Count());
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsOkWithList()
    {
        // Arrange
        var list = new TaskListDto
        {
            Id = 1,
            Name = "Test List",
            Tasks = new List<TaskDto>()
        };

        _serviceMock
            .Setup(s => s.GetListByIdAsync(1))
            .ReturnsAsync(list);

        // Act
        var result = await _sut.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<GetTaskListResponse>(okResult.Value);
        Assert.Equal("Test List", response.List!.Name);
    }

    [Fact]
    public async Task GetById_WhenNotExists_ReturnsNotFound()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.GetListByIdAsync(999))
            .ReturnsAsync((TaskListDto?)null);

        // Act
        var result = await _sut.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_WhenSuccessful_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateTaskListRequest { Name = "New List" };
        var createdList = new TaskListDto
        {
            Id = 1,
            Name = "New List",
            Tasks = new List<TaskDto>()
        };

        _serviceMock
            .Setup(s => s.CreateListAsync(request))
            .ReturnsAsync(createdList);

        // Act
        var result = await _sut.Create(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ListsController.GetById), createdResult.ActionName);
        var response = Assert.IsType<CreateTaskListResponse>(createdResult.Value);
        Assert.Equal("New List", response.List!.Name);
    }

    [Fact]
    public async Task Create_WhenDuplicateName_ReturnsConflict()
    {
        // Arrange
        var request = new CreateTaskListRequest { Name = "Existing List" };

        _serviceMock
            .Setup(s => s.CreateListAsync(request))
            .ThrowsAsync(new InvalidOperationException("A list with name 'Existing List' already exists."));

        // Act
        var result = await _sut.Create(request);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.NotNull(conflictResult.Value);
    }

    [Fact]
    public async Task Update_WhenSuccessful_ReturnsOkWithList()
    {
        // Arrange
        var request = new UpdateTaskListRequest { Name = "Updated Name" };
        var updatedList = new TaskListDto
        {
            Id = 1,
            Name = "Updated Name",
            Tasks = new List<TaskDto>()
        };

        _serviceMock
            .Setup(s => s.UpdateListAsync(1, request))
            .ReturnsAsync(updatedList);

        // Act
        var result = await _sut.Update(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<UpdateTaskListResponse>(okResult.Value);
        Assert.Equal("Updated Name", response.List!.Name);
    }

    [Fact]
    public async Task Update_WhenNotExists_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateTaskListRequest { Name = "New Name" };

        _serviceMock
            .Setup(s => s.UpdateListAsync(999, request))
            .ReturnsAsync((TaskListDto?)null);

        // Act
        var result = await _sut.Update(999, request);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Update_WhenDuplicateName_ReturnsConflict()
    {
        // Arrange
        var request = new UpdateTaskListRequest { Name = "Existing Name" };

        _serviceMock
            .Setup(s => s.UpdateListAsync(1, request))
            .ThrowsAsync(new InvalidOperationException("A list with name 'Existing Name' already exists."));

        // Act
        var result = await _sut.Update(1, request);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        Assert.NotNull(conflictResult.Value);
    }

    [Fact]
    public async Task Delete_WhenSuccessful_ReturnsNoContent()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.DeleteListAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNotExists_ReturnsNotFound()
    {
        // Arrange
        _serviceMock
            .Setup(s => s.DeleteListAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.Delete(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
