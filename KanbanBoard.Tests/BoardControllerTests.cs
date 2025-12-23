using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taskr.Controllers;
using Taskr.Data;
using Taskr.Models;
using Xunit;

namespace Taskr.KanbanBoard.Tests;

public class BoardControllerTests
{
    private KanbanDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<KanbanDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // fresh DB per test
            .Options;

        var ctx = new KanbanDbContext(options);

        // Seed minimal data
        var board = new Board { Title = "Demo Board" };
        board.Swimlanes.Add(new Swimlane { Name = "To Do", Position = 0 });
        ctx.Boards.Add(board);
        ctx.SaveChanges();

        return ctx;
    }

    [Fact]
    public async Task Index_Returns_View_With_Boards()
    {
        // Arrange
        await using var db = GetInMemoryContext();
        var controller = new BoardController(db);

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        Assert.IsType<ViewResult>(result);
        // Assert
        var model = viewResult.Model.Should().BeAssignableTo<Board>().Subject;
        Assert.IsType<Board>(model);
        
        model.Title.Should().Be("Demo Board");
        model.Swimlanes.Should().ContainSingle(c => c.Name == "To Do");
    }
}