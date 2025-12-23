using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Taskr.Models.User;
using Moq;
using Taskr.Data;
using Taskr.Services;
using Xunit;
using Microsoft.AspNetCore.Http;

namespace Taskr.KanbanBoard.Tests;

public class BoardServiceTests
{
    // Create a mock user manager that always returns the supplied user
    private static UserManager<AppUser> MockUserManager(AppUser user)
    {
        var store = new Mock<IUserStore<AppUser>>();
        var mgr = new Mock<UserManager<AppUser>>(
            store.Object,
            null, null, null, null, null, null, null, null);

        // GetUserAsync is called with the HttpContext's ClaimsPrincipal.
        // We'll make it ignore the principal and always return the supplied user.
        mgr.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        return mgr.Object;
    }

    private static IHttpContextAccessor MockHttpContextAccessor()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity()) // empty identity – fine for the mock
        };

        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.Setup(a => a.HttpContext).Returns(httpContext);
        return accessorMock.Object;
    }

    // Helper to spin up an in‑memory DbContext
    private static KanbanDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<KanbanDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB per test
            .Options;

        var ctx = new KanbanDbContext(options);
        // Ensure the schema is created (EF Core does this lazily, but we force it)
        ctx.Database.EnsureCreated();
        return ctx;
    }

    [Fact]
    public async Task GetOrCreateCurrentUserBoardAsync_CreatesBoard_WhenNoneExists()
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
        var userManager = MockUserManager(fakeUser);
        var httpContextAccessor = MockHttpContextAccessor();
        var db = CreateInMemoryDb();

        // Instantiate the service under test
        var boardService = new BoardService(db, userManager, httpContextAccessor);

        // ---------- Act ----------
        var board = await boardService.GetOrCreateCurrentUserKanbanBoardAsync();

        // ---------- Assert ----------
        // The method should have returned a non‑null board
        Assert.NotNull(board);
        // The board must belong to the fake user
        Assert.Equal(fakeUser.Id, board.OwnerId);
        // The title should contain the user name (as per service logic)
        Assert.Contains(fakeUser.UserName, board.Title);
        // The board should have the default swimlanes
        Assert.Equal(3, board.Swimlanes.Count);
        // The board should now be persisted in the in‑memory DB
        var boardFromDb = await db.Boards.FirstOrDefaultAsync(b => b.OwnerId == fakeUser.Id);
        Assert.NotNull(boardFromDb);
        Assert.Equal(board.Id, boardFromDb!.Id);
    }
}