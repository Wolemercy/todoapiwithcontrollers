namespace TodoApi.Tests;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using TodoApiWithControllers.Models;
using TodoApiWithControllers.Services;
using TodoApiWithControllers.Controllers;

public class UnitTest1
{
    [Fact]
    public async void PostTodoItem_CreatesATodoItem()
    {
        var mockTodoItemDTO = new Mock<TodoItemDTO>();
        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.CreateTodoItem(It.IsAny<TodoItemDTO>()))
            .ReturnsAsync(GetTodos()[0]);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.PostTodoItem(mockTodoItemDTO.Object);

        mockTodoService.Verify(svc => svc.CreateTodoItem(It.IsAny<TodoItemDTO>()), Times.Once);
    }

    [Fact]
    public async void GetTodoItem_ReturnsATodoItem()
    {
        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.GetTodoItemById(It.IsAny<long>()))
            .ReturnsAsync(GetTodos()[0]);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.GetTodoItem(1);

        mockTodoService.Verify(svc => svc.GetTodoItemById(It.IsAny<long>()), Times.Once);
    }

    [Fact]
    public async void GetTodoItem_ReturnsAllTodoItems()
    {
        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.GetTodoItems())
            .ReturnsAsync(GetTodos());

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.GetTodoItem(1);

        mockTodoService.Verify(svc => svc.GetTodoItems(), Times.Once);
    }

    [Fact]
    public async void PutTodoItem_UpdatesATodoItemA()
    {
        var mockTodoItemDTO = new Mock<TodoItemDTO>();
        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.UpdateTodoItem(It.IsAny<long>(), It.IsAny<TodoItemDTO>()))
            .ReturnsAsync(true);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.PutTodoItem(1, mockTodoItemDTO.Object);

        mockTodoService.Verify(svc => svc.UpdateTodoItem(It.IsAny<long>(), It.IsAny<TodoItemDTO>()), Times.Once);
    }

    [Fact]
    public async void PatchTodoItem_PartiallyUpdatesATodoItemA()
    {
        var mockTodoItemUpdateDTO = new Mock<TodoItemUpdateDTO>();
        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.PartiallyUpdateTodoItem(It.IsAny<long>(), It.IsAny<TodoItemUpdateDTO>()))
            .ReturnsAsync(true);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.PatchTodoItem(1, mockTodoItemUpdateDTO.Object);

        mockTodoService.Verify(svc => svc.PartiallyUpdateTodoItem(It.IsAny<long>(),It.IsAny<TodoItemUpdateDTO>()), Times.Once);
    }

    [Fact]
    public async void DeleteTodoItem_DeletesATodoItem()
    {
        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(svc => svc.DeleteTodoItem(It.IsAny<long>()))
            .ReturnsAsync(true);

        var controller = new TodoItemsController(mockTodoService.Object);

        var result = await controller.DeleteTodoItem(1);

        mockTodoService.Verify(svc => svc.DeleteTodoItem(It.IsAny<long>()), Times.Once);
    }

    private List<TodoItemDTO> GetTodos()
    {
        var todos = new List<TodoItemDTO>();
        todos.Add(new TodoItemDTO()
        {
            Id = 1,
            Name = "Task 1",
            IsCompleted = true
        });

        todos.Add(new TodoItemDTO()
        {
            Id = 2,
            Name = "Task 2",
            IsCompleted = false
        });

        return todos;
    }
}