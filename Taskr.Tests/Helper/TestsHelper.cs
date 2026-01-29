using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Moq;
using Taskr.Models.User;

namespace Taskr.Tests.Helper;

public static class TestsHelper
{
    /// Create a mock user manager that always returns the supplied user
    public static UserManager<AppUser> MockUserManager(AppUser user)
    {
        var store = new Mock<IUserStore<AppUser>>();
        var mgr = new Mock<UserManager<AppUser>>(
            store.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        // GetUserAsync is called with the HttpContext's ClaimsPrincipal.
        // We'll make it ignore the principal and always return the supplied user.
        mgr.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        return mgr.Object;
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
}