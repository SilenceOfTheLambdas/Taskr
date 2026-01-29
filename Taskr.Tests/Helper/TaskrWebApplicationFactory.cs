using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Taskr.Data;
using Taskr.Models;
using Taskr.Models.User;
using Taskr.Services;

namespace Taskr.Tests.Helper;

public class TaskrWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();
    private readonly SemaphoreSlim _lock = new(1, 1);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(Directory.GetCurrentDirectory());
            
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => 
                d.ServiceType == typeof(DbContextOptions<KanbanDbContext>));
            if (descriptor != null) services.Remove(descriptor);
            
            services.AddDbContext<KanbanDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
            
            // Add the test authentication scheme
            services.AddAuthentication(options =>
                {
                    // Need to make sure our default scheme is set to the test scheme
                    // so that the ChallengeAsync method in the controller will work correctly
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                    options.DefaultScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestsAuthHandler>("Test", _ => { });
                
            // Mock user manager to return a new user corresponding to the test claims
            var testUser = TestsHelper.CreateTestUser();
            testUser.Id = "test-user-id";
            testUser.UserName = "Test User";
            services.AddScoped(_ => TestsHelper.MockUserManager(testUser));
        });
    }
    
    //Helper method to ensure seeding of user data
    public async Task SeedTestDataAsync()
    {
        await _lock.WaitAsync();
        try
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<KanbanDbContext>();
            //Ensure the database is created
            await db.Database.EnsureCreatedAsync();
            
            const string testUserId = "test-user-id";
            var user = await db.Users.Include(u => u.Board)
                .FirstOrDefaultAsync(u => u.Id == testUserId);

            if (user == null)
            {
                var testUser = TestsHelper.CreateTestUser();
                testUser.Id = testUserId;
                testUser.UserName = "Test User";
                db.Users.Add(testUser);
                await db.SaveChangesAsync();
            }
            
            // Ensure a board exists for this user so tests can use it's IDs
            var board = await db.Boards.FirstOrDefaultAsync(b => b.OwnerId == testUserId);
            if (board == null)
            {
                board = new Board
                { Title = "Test Board",
                    OwnerId = testUserId };
                db.Boards.Add(board);
                await db.SaveChangesAsync();
                
                db.Swimlanes.Add(new Swimlane { Name = "Backlog", Board = board });
                db.Swimlanes.Add(new Swimlane { Name = "In Progress", Board = board });
                db.Swimlanes.Add(new Swimlane { Name = "Completed", Board = board });
                await db.SaveChangesAsync();
            }
        }
        finally
        {
            _lock.Release();
        }
    }
}