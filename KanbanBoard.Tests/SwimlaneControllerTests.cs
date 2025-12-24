using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Taskr.Models.User;
using Moq;
using Taskr.Data;
using Taskr.Models;
using Taskr.Services;
using Xunit;

namespace Taskr.KanbanBoard.Tests;

public class SwimlaneControllerTests
{
    [Fact]
    public async Task TestSwimlaneCreation()
    {
        // ---------- Arrange ----------
        // Create a fake user that will act as the logged‑in user
        var fakeUser = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "test.user@example.com",
            Email = "test.user@example.com",
            EmailConfirmed = true
        };

        // Mock the dependencies
        var userManager = TestsHelper.MockUserManager(fakeUser);
        var httpContextAccessor = TestsHelper.MockHttpContextAccessor();
        var db = TestsHelper.CreateInMemoryDb();

        // Instantiate the service under test
        var boardService = new BoardController(db, userManager, httpContextAccessor);

        // ---------- Act ----------
        var board = await boardService.GetOrCreateCurrentUserKanbanBoardAsync();
        
        // Make sure we actually got a board back
        Assert.NotNull(board);
        
        var newSwimlane = new Swimlane()
        {
            Board = board,
            BoardId = board.Id,
            Name = "Test Swimlane",
            Position = board.Swimlanes.LastOrDefault()!.Position + 1
        };
        db.Add(newSwimlane);
        await db.SaveChangesAsync();
        
        // Assert that the swimlane was created successfully
        Assert.Equal(4, board.Swimlanes.Count);
        
        // and then make sure the last swimlane has the expected name
        Assert.Equal("Test Swimlane", board.Swimlanes.Last().Name);
    }
}