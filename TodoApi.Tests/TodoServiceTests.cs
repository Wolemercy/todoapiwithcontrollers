namespace TodoApi.Tests;

using System.Linq.Expressions;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using TodoApiWithControllers.Models;
using TodoApiWithControllers.Services;
using Xunit;

public class TodoServiceTests
{
    [Fact]
    public async void CreateTodoItem_ReturnsANewlyCreatedTodoItem()
    {

        var expectedResult = new Faker<TodoItemDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(0, 0))
            .RuleFor(u => u.Name, f => f.Hacker.Verb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool())
            .Generate();
    
        var mockSet = new Mock<DbSet<TodoItem>>();

        var options = new DbContextOptions<TodoContext>();
        var mockTodoContext = new Mock<TodoContext>(options);
        mockTodoContext.Setup(ctx => ctx.Todo).Returns(mockSet.Object);
        mockTodoContext.Setup(ctx => ctx.SaveChangesAsync(CancellationToken.None));

        var service = new TodoService(mockTodoContext.Object);

        var actualResult = await service.CreateTodoItem(expectedResult);

        mockSet.Verify(ctx => ctx.Add(It.IsAny<TodoItem>()), Times.Once);
        mockTodoContext.Verify(ctx => ctx.SaveChangesAsync(CancellationToken.None), Times.Once);

        Assert.IsType<TodoItemDTO>(actualResult);
        Assert.NotNull(actualResult);

        var actualResultJSON = JsonConvert.SerializeObject(actualResult);
        var expectedResultJSON = JsonConvert.SerializeObject(expectedResult);
        Assert.Equal(actualResultJSON, expectedResultJSON);
    }

    [Fact]
    public async void GetTodoItemById_ReturnsATodoItem()
    {
        var expectedResult = new Faker<TodoItem>()
            .RuleFor(u => u.Id, f => f.Random.Long(0, 0))
            .RuleFor(u => u.Name, f => f.Hacker.Verb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool())
            .Generate();

        var mockSet = new Mock<DbSet<TodoItem>>();
        mockSet.Setup(ctx => ctx.FindAsync(It.IsAny<long>())).ReturnsAsync(expectedResult);

        var options = new DbContextOptions<TodoContext>();
        var mockTodoContext = new Mock<TodoContext>(options);
        mockTodoContext.Setup(ctx => ctx.Todo).Returns(mockSet.Object);

        var service = new TodoService(mockTodoContext.Object);

        var actualResult = await service.GetTodoItemById(expectedResult.Id);

        mockSet.Verify(ctx => ctx.FindAsync(It.IsAny<long>()), Times.Once);

        Assert.IsType<TodoItemDTO>(actualResult);
        Assert.NotNull(actualResult);

        Assert.Equal(actualResult.Id, expectedResult.Id);
        Assert.Equal(actualResult.Name, expectedResult.Name);
        Assert.Equal(actualResult.IsCompleted, expectedResult.IsCompleted);
    }

    [Fact]
    public async void GetTodoItemById_ReturnsNull_TodoItemDoesNotExist()
    {

        var mockSet = new Mock<DbSet<TodoItem>>();
        mockSet.Setup(ctx => ctx.FindAsync(It.IsAny<long>())).ReturnsAsync(() => null);

        var options = new DbContextOptions<TodoContext>();
        var mockTodoContext = new Mock<TodoContext>(options);
        mockTodoContext.Setup(ctx => ctx.Todo).Returns(mockSet.Object);

        var service = new TodoService(mockTodoContext.Object);

        var actualResult = await service.GetTodoItemById(0);

        mockSet.Verify(ctx => ctx.FindAsync(It.IsAny<long>()), Times.Once);
        Assert.Null(actualResult);
    }

    [Fact]
    public async void GetTodoItems_ReturnsAllTodoItems()
    {
        var expectedResult = new Faker<TodoItem>()
            .RuleFor(u => u.Id, f => f.Random.Long(0, 0))
            .RuleFor(u => u.Name, f => f.Hacker.Verb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool())
            .Generate(3);

        var data = expectedResult.AsQueryable();

        var mockSet = new Mock<DbSet<TodoItem>>();
        mockSet.As<IQueryable<TodoItem>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<TodoItem>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<TodoItem>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<TodoItem>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

        var options = new DbContextOptions<TodoContext>();
        var mockTodoContext = new Mock<TodoContext>(options);
        mockTodoContext.Setup(ctx => ctx.Todo).Returns(mockSet.Object);

        var service = new TodoService(mockTodoContext.Object);

        var actualResult = await service.GetTodoItems();

        Assert.NotNull(actualResult);
        Assert.Equal(3, actualResult.Count());

        var actualResultList = actualResult.ToList();

        Assert.Equal(actualResultList[0].Name, expectedResult[0].Name);
        Assert.Equal(actualResultList[1].Name, expectedResult[1].Name);
        Assert.Equal(actualResultList[2].Name, expectedResult[2].Name);
    }

    [Fact]
    public async void UpdateTodoItem_ReturnsATrueBoolean()
    {

        var originalTodoItem = new Faker<TodoItem>()
            .RuleFor(u => u.Id, f => f.Random.Long(0, 0))
            .RuleFor(u => u.Name, f => f.Hacker.Verb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool())
            .Generate();

        var todoItemUpdate = new Faker<TodoItemDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(originalTodoItem.Id, originalTodoItem.Id))
            .RuleFor(u => u.Name, f => f.Hacker.Verb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool())
            .Generate();

        var mockSet = new Mock<DbSet<TodoItem>>();
        mockSet.Setup(ctx => ctx.FindAsync(It.IsAny<long>())).ReturnsAsync(originalTodoItem);

        var options = new DbContextOptions<TodoContext>();
        var mockTodoContext = new Mock<TodoContext>(options);
        mockTodoContext.Setup(ctx => ctx.Todo).Returns(mockSet.Object);
        mockTodoContext.Setup(ctx => ctx.SaveChangesAsync(CancellationToken.None));

        var service = new TodoService(mockTodoContext.Object);

        var actualResult = await service.UpdateTodoItem(originalTodoItem.Id, todoItemUpdate);

        mockSet.Verify(ctx => ctx.FindAsync(It.IsAny<long>()), Times.Once);
        mockTodoContext.Verify(ctx => ctx.SaveChangesAsync(CancellationToken.None), Times.Once);

        Assert.IsType<Boolean>(actualResult);
        Assert.True(actualResult);
    }

    [Fact]
    public async void UpdateTodoItem_ReturnsAFalseBoolean_TodoItemDoesNotExist()
    {

        var todoItemUpdate = new Faker<TodoItemDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(1, 1))
            .RuleFor(u => u.Name, f => f.Hacker.Verb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool())
            .Generate();

        var mockSet = new Mock<DbSet<TodoItem>>();
        mockSet.Setup(ctx => ctx.FindAsync(It.IsAny<long>())).ReturnsAsync(() => null);

        var options = new DbContextOptions<TodoContext>();
        var mockTodoContext = new Mock<TodoContext>(options);
        mockTodoContext.Setup(ctx => ctx.Todo).Returns(mockSet.Object);
        mockTodoContext.Setup(ctx => ctx.SaveChangesAsync(CancellationToken.None));

        var service = new TodoService(mockTodoContext.Object);

        var actualResult = await service.UpdateTodoItem(1, todoItemUpdate);

        mockSet.Verify(ctx => ctx.FindAsync(It.IsAny<long>()), Times.Once);
        mockTodoContext.Verify(ctx => ctx.SaveChangesAsync(CancellationToken.None), Times.Never);

        Assert.IsType<Boolean>(actualResult);
        Assert.False(actualResult);
    }

    [Fact]
    public async void PartiallyUpdateTodoItem_ReturnsATrueBoolean()
    {

        var originalTodoItem = new Faker<TodoItem>()
            .RuleFor(u => u.Id, f => f.Random.Long(0, 0))
            .RuleFor(u => u.Name, f => f.Hacker.Verb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool())
            .Generate();

        var todoItemUpdate = new Faker<TodoItemUpdateDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(originalTodoItem.Id, originalTodoItem.Id))
            .RuleFor(u => u.Name, f => f.Hacker.Verb())
            .Generate();

        var mockSet = new Mock<DbSet<TodoItem>>();
        mockSet.Setup(ctx => ctx.FindAsync(It.IsAny<long>())).ReturnsAsync(originalTodoItem);

        var options = new DbContextOptions<TodoContext>();
        var mockTodoContext = new Mock<TodoContext>(options);
        mockTodoContext.Setup(ctx => ctx.Todo).Returns(mockSet.Object);
        mockTodoContext.Setup(ctx => ctx.SaveChangesAsync(CancellationToken.None));

        var service = new TodoService(mockTodoContext.Object);

        var actualResult = await service.PartiallyUpdateTodoItem(originalTodoItem.Id, todoItemUpdate);

        mockSet.Verify(ctx => ctx.FindAsync(It.IsAny<long>()), Times.Once);
        mockTodoContext.Verify(ctx => ctx.SaveChangesAsync(CancellationToken.None), Times.Once);

        Assert.IsType<Boolean>(actualResult);
        Assert.True(actualResult);
    }

    [Fact]
    public async void PartiallyUpdateTodoItem_ReturnsAFalseBoolean_TodoItemDoesNotExist()
    {

        var todoItemUpdate = new Faker<TodoItemUpdateDTO>()
            .RuleFor(u => u.Id, f => f.Random.Long(1, 1))
            .RuleFor(u => u.Name, f => f.Hacker.Verb())
            .Generate();

        var mockSet = new Mock<DbSet<TodoItem>>();
        mockSet.Setup(ctx => ctx.FindAsync(It.IsAny<long>())).ReturnsAsync(() => null);

        var options = new DbContextOptions<TodoContext>();
        var mockTodoContext = new Mock<TodoContext>(options);
        mockTodoContext.Setup(ctx => ctx.Todo).Returns(mockSet.Object);
        mockTodoContext.Setup(ctx => ctx.SaveChangesAsync(CancellationToken.None));

        var service = new TodoService(mockTodoContext.Object);

        var actualResult = await service.PartiallyUpdateTodoItem(1, todoItemUpdate);

        mockSet.Verify(ctx => ctx.FindAsync(It.IsAny<long>()), Times.Once);
        mockTodoContext.Verify(ctx => ctx.SaveChangesAsync(CancellationToken.None), Times.Never);

        Assert.IsType<Boolean>(actualResult);
        Assert.False(actualResult);
    }

    [Fact]
    public async void DeleteTodoItem_ReturnsATrueBoolean()
    {

        var originalTodoItem = new Faker<TodoItem>()
            .RuleFor(u => u.Id, f => f.Random.Long(0, 0))
            .RuleFor(u => u.Name, f => f.Hacker.Verb())
            .RuleFor(u => u.IsCompleted, f => f.Random.Bool())
            .Generate();

        var mockSet = new Mock<DbSet<TodoItem>>();
        mockSet.Setup(ctx => ctx.FindAsync(It.IsAny<long>())).ReturnsAsync(originalTodoItem);

        var options = new DbContextOptions<TodoContext>();
        var mockTodoContext = new Mock<TodoContext>(options);
        mockTodoContext.Setup(ctx => ctx.Todo).Returns(mockSet.Object);
        mockTodoContext.Setup(ctx => ctx.SaveChangesAsync(CancellationToken.None));

        var service = new TodoService(mockTodoContext.Object);

        var actualResult = await service.DeleteTodoItem(originalTodoItem.Id);

        mockSet.Verify(ctx => ctx.FindAsync(It.IsAny<long>()), Times.Once);
        mockSet.Verify(ctx => ctx.Remove(It.IsAny<TodoItem>()), Times.Once);
        mockTodoContext.Verify(ctx => ctx.SaveChangesAsync(CancellationToken.None), Times.Once);

        Assert.IsType<Boolean>(actualResult);
        Assert.True(actualResult);
    }

    [Fact]
    public async void DeleteTodoItem_ReturnsAFalseBoolean_TodoItemDoesNotExist()
    {
        var mockSet = new Mock<DbSet<TodoItem>>();
        mockSet.Setup(ctx => ctx.FindAsync(It.IsAny<long>())).ReturnsAsync(() => null);

        var options = new DbContextOptions<TodoContext>();
        var mockTodoContext = new Mock<TodoContext>(options);
        mockTodoContext.Setup(ctx => ctx.Todo).Returns(mockSet.Object);
        mockTodoContext.Setup(ctx => ctx.SaveChangesAsync(CancellationToken.None));

        var service = new TodoService(mockTodoContext.Object);

        var actualResult = await service.DeleteTodoItem(100);

        mockSet.Verify(ctx => ctx.FindAsync(It.IsAny<long>()), Times.Once);
        mockSet.Verify(ctx => ctx.Remove(It.IsAny<TodoItem>()), Times.Never);
        mockTodoContext.Verify(ctx => ctx.SaveChangesAsync(CancellationToken.None), Times.Never);

        Assert.IsType<Boolean>(actualResult);
        Assert.False(actualResult);
    }

}

