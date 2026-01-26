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

    [Fact]
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

    [Fact]
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
}