using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Taskr.Services;
using Taskr.Tests.Helper;

namespace Taskr.Tests;

public class BoardServiceTests(TaskrWebApplicationFactory factory) : IClassFixture<TaskrWebApplicationFactory>
{
    [Fact(DisplayName = "Board is created for new users")]
    public async Task GetOrCreateCurrentUserKanbanBoardAsync_ReturnsBoard()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext();
        
        var boardService = scope.ServiceProvider.GetRequiredService<BoardService>();

        // Act
        var board = await boardService.GetOrCreateCurrentUserKanbanBoardAsync();
        
        // Assert
        board.Should().NotBeNull();
    }

    [Fact(DisplayName = "New board has 3 swimlanes by default")]
    public async Task GetOrCreateCurrentUserKanbanBoardAsync_Returns3Swimlanes()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext();
        
        var boardService = scope.ServiceProvider.GetRequiredService<BoardService>();

        // Act
        var board = await boardService.GetOrCreateCurrentUserKanbanBoardAsync();
        
        board.Should().NotBeNull();
        
        // Assert
        board.Swimlanes.Count.Should().Be(3);
    }
    
    [Fact(DisplayName = "Swimlane 1 has 2 cards by default")]
    public async Task GetOrCreateCurrentUserKanbanBoardAsync_Returns2CardsInSwimlane1()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext();
        
        var boardService = scope.ServiceProvider.GetRequiredService<BoardService>();

        // Act
        var board = await boardService.GetOrCreateCurrentUserKanbanBoardAsync();
        
        board.Should().NotBeNull();
        board.Swimlanes.Count.Should().Be(3);
        
        var swimlane1 = board.Swimlanes.First();
        
        // Assert
        swimlane1.Cards.Count.Should().Be(2);
    }
}