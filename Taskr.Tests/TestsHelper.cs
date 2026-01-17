using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Taskr.Data;
using Taskr.Models.User;
using Taskr.Services;

namespace Taskr.Tests;

public static class TestsHelper
{
    /// Create a mock user manager that always returns the supplied user
    public static UserManager<AppUser> MockUserManager(AppUser user)
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

    public static IHttpContextAccessor MockHttpContextAccessor()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity()) // empty identity – fine for the mock
        };

        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.Setup(a => a.HttpContext).Returns(httpContext);
        return accessorMock.Object;
    }

    /// Helper to spin up an in‑memory DbContext
    public static KanbanDbContext CreateInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<KanbanDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB per test
            .Options;

        var ctx = new KanbanDbContext(options);
        // Ensure the schema is created (EF Core does this lazily, but we force it)
        ctx.Database.EnsureCreated();
        return ctx;
    }

    /// <summary>
    /// Creates a test user with a username and email.
    /// </summary>
    /// <returns></returns>
    public static AppUser CreateTestUser()
    {
        return new AppUser
        {
            Id = "test-user-id",
            UserName = "test.user@example.com",
            Email = "test.user@example.com",
            EmailConfirmed = true
        };
    }
    
    public static void SetupEnvironment(out AppUser fakeUser, out UserManager<AppUser> userManager,
        out IHttpContextAccessor httpContextAccessor, out KanbanDbContext db, out BoardService boardService)
    {
        // ---------- Arrange ----------
        // Create a fake user that will act as the logged‑in user
        fakeUser = CreateTestUser();

        // Mock the dependencies
        userManager = MockUserManager(fakeUser);
        httpContextAccessor = MockHttpContextAccessor();
        db = CreateInMemoryDb();

        // Instantiate the service under test
        boardService = new BoardService(db, userManager, httpContextAccessor);
    }
}