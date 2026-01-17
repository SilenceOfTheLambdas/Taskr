using Taskr.Models;
using Taskr.Services;

namespace Taskr.Tests;

public class SwimlaneControllerTests
{
    [Fact]
    public async Task TestSwimlaneCreation()
    {
        // ---------- Arrange ----------
        // Create a fake user that will act as the logged‑in user
        var fakeUser = TestsHelper.CreateTestUser();

        // Mock the dependencies
        var userManager = TestsHelper.MockUserManager(fakeUser);
        var httpContextAccessor = TestsHelper.MockHttpContextAccessor();
        var db = TestsHelper.CreateInMemoryDb();

        // Instantiate the service under test
        var boardService = new BoardService(db, userManager, httpContextAccessor);

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