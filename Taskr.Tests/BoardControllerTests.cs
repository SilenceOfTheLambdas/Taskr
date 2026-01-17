using Microsoft.EntityFrameworkCore;
using Taskr.Services;

namespace Taskr.Tests;

public class BoardControllerTests
{
    [Fact]
    public async Task GetOrCreateCurrentUserBoardAsync_CreatesBoard_WhenNoneExists()
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

        // ---------- Assert ----------
        // The method should have returned a non‑null board
        Assert.NotNull(board);
        // The board must belong to the fake user
        Assert.Equal(fakeUser.Id, board.OwnerId);
        // The title should contain the username (as per service logic)
        Assert.Contains(fakeUser.UserName, board.Title);
        // The board should have the default swimlanes
        Assert.Equal(3, board.Swimlanes.Count);
        // The board should now be persisted in the in‑memory DB
        var boardFromDb = await db.Boards.FirstOrDefaultAsync(b => b.OwnerId == fakeUser.Id);
        Assert.NotNull(boardFromDb);
        Assert.Equal(board.Id, boardFromDb!.Id);
    }
}