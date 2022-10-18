namespace TodoApi.Tests;
using Xunit;
using Moq;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using TodoApiWithControllers.Models;
using TodoApiWithControllers.Services;
using TodoApiWithControllers.Controllers;

public class TodoItemsControllerTests
{
    [Fact]
    public async void PostTodoItem_ReturnsCreatedAtActionResult_CreatesATodoItem()
    {

        var expectedResult = new Faker<TodoItemDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(1, 1))
            .RuleFor(u => u.Name, f => f.Hacker.IngVerb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool())
            .Generate();
        
        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.CreateTodoItem(It.IsAny<TodoItemDTO>()))
            .ReturnsAsync(expectedResult);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.PostTodoItem(expectedResult);

        mockTodoService.Verify(svc => svc.CreateTodoItem(It.IsAny<TodoItemDTO>()), Times.Once);

        Assert.IsType<CreatedAtActionResult>(result.Result);
        var actualResult = (result.Result as CreatedAtActionResult)?.Value!;

        Assert.NotNull(actualResult);
        Assert.Equal(actualResult, expectedResult);

    }

    [Fact]
    public async void PostTodoItem_ReturnsBadRequest_FailedToCreateTodoItem()
    {

        var todoItemCreateStub = new Faker<TodoItemDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(1, 1))
            .RuleFor(u => u.Name, f => f.Hacker.IngVerb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool())
            .Generate();
        
        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.CreateTodoItem(It.IsAny<TodoItemDTO>()))
            .ReturnsAsync(() => null);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.PostTodoItem(todoItemCreateStub);

        mockTodoService.Verify(svc => svc.CreateTodoItem(It.IsAny<TodoItemDTO>()), Times.Once);
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async void GetTodoItem_ReturnsOkObjectResult_GetsATodoItem()
    {
        var expectedResult = new Faker<TodoItemDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(1, 1))
            .RuleFor(u => u.Name, f => f.Hacker.IngVerb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool())
            .Generate();

        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.GetTodoItemById(It.IsAny<long>()))
            .ReturnsAsync(expectedResult);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.GetTodoItem(1);

        mockTodoService.Verify(svc => svc.GetTodoItemById(It.IsAny<long>()), Times.Once);

        Assert.IsType<OkObjectResult>(result.Result);
        var actualResult = (result.Result as OkObjectResult)?.Value!;

        Assert.NotNull(actualResult);
        Assert.Equal(actualResult, expectedResult);
    }

    [Fact]
    public async void GetTodoItem_ReturnsNotFoundResult_FailedToGetTodoItem()
    {
        var expectedResult = new Faker<TodoItemDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(1, 1))
            .RuleFor(u => u.Name, f => f.Hacker.IngVerb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool())
            .Generate();

        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.GetTodoItemById(It.IsAny<long>()))
            .ReturnsAsync(() => null);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.GetTodoItem(1);

        mockTodoService.Verify(svc => svc.GetTodoItemById(It.IsAny<long>()), Times.Once);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async void GetTodoItems_ReturnsOkObjectResult_GetsAllTodoItems()
    {
        var expectedResult = new Faker<TodoItemDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(1, 10))
            .RuleFor(u => u.Name, f => f.Hacker.IngVerb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool())
            .Generate(2);

        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.GetTodoItems())
            .ReturnsAsync(expectedResult);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.GetTodoItems();

        mockTodoService.Verify(svc => svc.GetTodoItems(), Times.Once);

        Assert.IsType<OkObjectResult>(result.Result);
        var actualResult = (result.Result as OkObjectResult)?.Value!;

        Assert.NotNull(actualResult);
        Assert.Equal(actualResult, expectedResult);
    }

    [Fact]
    public async void PutTodoItem_ReturnsNoContentResult_UpdatesATodoItem()
    {
        var todoItemFaker = new Faker<TodoItemDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(1, 1))
            .RuleFor(u => u.Name, f => f.Hacker.IngVerb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool());

        var todoItemStub = todoItemFaker.Generate();

        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.UpdateTodoItem(It.IsAny<long>(), It.IsAny<TodoItemDTO>()))
            .ReturnsAsync(true);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.PutTodoItem(1, todoItemStub);

        mockTodoService.Verify(svc => svc.UpdateTodoItem(It.IsAny<long>(), It.IsAny<TodoItemDTO>()), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async void PutTodoItem_ReturnsBadRequest_IdInQueryDiffersFromIdInBody()
    {
        var todoItemFaker = new Faker<TodoItemDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(1, 1))
            .RuleFor(u => u.Name, f => f.Hacker.IngVerb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool());

        var todoItemStub = todoItemFaker.Generate();

        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.UpdateTodoItem(It.IsAny<long>(), It.IsAny<TodoItemDTO>()))
            .ReturnsAsync(true);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.PutTodoItem(100, todoItemStub);

        mockTodoService.Verify(svc => svc.UpdateTodoItem(It.IsAny<long>(), It.IsAny<TodoItemDTO>()), Times.Never);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async void PutTodoItem_ReturnsNotFound_TodoItemNotFound()
    {
        var todoItemFaker = new Faker<TodoItemDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(1, 1))
            .RuleFor(u => u.Name, f => f.Hacker.IngVerb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool());

        var todoItemStub = todoItemFaker.Generate();

        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.UpdateTodoItem(It.IsAny<long>(), It.IsAny<TodoItemDTO>()))
            .ReturnsAsync(false);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.PutTodoItem(1, todoItemStub);

        mockTodoService.Verify(svc => svc.UpdateTodoItem(It.IsAny<long>(), It.IsAny<TodoItemDTO>()), Times.Once);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async void PatchTodoItem_ReturnsNoContentResult_PartiallyUpdatesATodoItem()
    {
        var todoItemUpdateFaker = new Faker<TodoItemUpdateDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(1, 1))
            .RuleFor(u => u.Name, f => f.Hacker.IngVerb());

        var todoItemStub = todoItemUpdateFaker.Generate();

        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.PartiallyUpdateTodoItem(It.IsAny<long>(), It.IsAny<TodoItemUpdateDTO>()))
            .ReturnsAsync(true);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.PatchTodoItem(1, todoItemStub);

        mockTodoService.Verify(svc => svc.PartiallyUpdateTodoItem(It.IsAny<long>(),It.IsAny<TodoItemUpdateDTO>()), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async void PatchTodoItem_ReturnsBadRequestResult_IdInQueryDiffersFromIdInBody()
    {
        var todoItemUpdateFaker = new Faker<TodoItemUpdateDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(1, 1))
            .RuleFor(u => u.Name, f => f.Hacker.IngVerb());

        var todoItemStub = todoItemUpdateFaker.Generate();

        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.PartiallyUpdateTodoItem(It.IsAny<long>(), It.IsAny<TodoItemUpdateDTO>()))
            .ReturnsAsync(true);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.PatchTodoItem(100, todoItemStub);

        mockTodoService.Verify(svc => svc.PartiallyUpdateTodoItem(It.IsAny<long>(), It.IsAny<TodoItemUpdateDTO>()), Times.Never);
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async void PatchTodoItem_ReturnsNotFoundResult_TodoItemNotFound()
    {
        var todoItemUpdateFaker = new Faker<TodoItemUpdateDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(1, 1))
            .RuleFor(u => u.Name, f => f.Hacker.IngVerb());

        var todoItemStub = todoItemUpdateFaker.Generate();

        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.PartiallyUpdateTodoItem(It.IsAny<long>(), It.IsAny<TodoItemUpdateDTO>()))
            .ReturnsAsync(false);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.PatchTodoItem(1, todoItemStub);

        mockTodoService.Verify(svc => svc.PartiallyUpdateTodoItem(It.IsAny<long>(), It.IsAny<TodoItemUpdateDTO>()), Times.Once);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async void DeleteTodoItem_ReturnsNoContentResult_DeletesATodoItem()
    {
        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.DeleteTodoItem(It.IsAny<long>()))
            .ReturnsAsync(true);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.DeleteTodoItem(1);

        mockTodoService.Verify(svc => svc.DeleteTodoItem(It.IsAny<long>()), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async void DeleteTodoItem_ReturnsNotFoundResult_TodoItemNotFound()
    {
        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.DeleteTodoItem(It.IsAny<long>()))
            .ReturnsAsync(false);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.DeleteTodoItem(1);

        mockTodoService.Verify(svc => svc.DeleteTodoItem(It.IsAny<long>()), Times.Once);
        Assert.IsType<NotFoundResult>(result);
    }

}