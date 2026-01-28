using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Taskr.Data;
using Taskr.Tests.Helper;

namespace Taskr.Tests;

public class SwimlaneControllerTests(TaskrWebApplicationFactory factory) : IClassFixture<TaskrWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false
    });

    [Fact(DisplayName = "Create New Swimlane")]
    public async Task CreateNewSwimlane_ReturnsCreated_AndSavesToDb()
    {
        // Arrange
        await factory.SeedTestDataAsync();
        const string newSwimlaneName = "Testing Lane";
        
        // Act
        var response = await _client.PostAsync($"Swimlane/CreateNewSwimlane?swimlaneName={newSwimlaneName}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // Verify database state
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<KanbanDbContext>();
        var swimlane = await db.Swimlanes.FirstOrDefaultAsync(s => s.Name == newSwimlaneName);
        
        swimlane.Should().NotBeNull();
        swimlane.Name.Should().Be(newSwimlaneName);
    }

    [Fact(DisplayName = "Delete Swimlane")]
    public async Task DeleteSwimlane_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        await factory.SeedTestDataAsync();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<KanbanDbContext>();
        var swimlane = await db.Swimlanes.FirstAsync();
        
        var swimlaneId = swimlane.Id;
        
        // Act
        var response = await _client.DeleteAsync($"Swimlane/DeleteSwimlane?swimlaneId={swimlaneId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify database state
        using var verifyScope = factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<KanbanDbContext>();
        (await verifyDb.Swimlanes.AnyAsync(s => s.Id == swimlaneId)).Should().BeFalse();
    }

    [Fact(DisplayName = "Update Swimlane Position")]
    public async Task UpdateSwimlanePosition_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        await factory.SeedTestDataAsync();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<KanbanDbContext>();

        const string testUserId = "test-user-id";
        var board = await db.Boards.Include(b => b.Swimlanes)
            .FirstOrDefaultAsync(b => b.OwnerId == testUserId);

        // Clear existing cards to ensure test isolation
        db.Cards.RemoveRange(db.Cards);
        await db.SaveChangesAsync();

        var s1 = board!.Swimlanes.First(); // starting pos 0
        var s2 = board.Swimlanes.Last(); // starting pos 1
        
        var response = await _client.PatchAsync($"Swimlane/UpdateSwimlanePosition?swimlaneId={s1.Id}&newPosition={1}", null);
        
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Reload data by creating a new scope to avoid cache
        using var verifyScope = factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<KanbanDbContext>();
        
        // Check that the position of s1 has been updated
        verifyDb.Swimlanes.First(s => s.Id == s1.Id).Position.Should().Be(2);
        
        // Similarly, check that the position of s2 has been updated
        verifyDb.Swimlanes.First(s => s.Id == s2.Id).Position.Should().Be(1);
    }
}