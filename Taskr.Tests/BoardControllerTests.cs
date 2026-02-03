using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Taskr.Data;
using Taskr.Models.Tag;
using Taskr.Tests.Helper;

namespace Taskr.Tests;

public class BoardControllerTests(TaskrWebApplicationFactory factory) : IClassFixture<TaskrWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false
    });

    [Fact(DisplayName = "Create New Tag")]
    public async Task CreateNewTag_ReturnsSuccess()
    {
        // Arrange
        // Seed the user once
        await factory.SeedTestDataAsync();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<KanbanDbContext>();
        
        const string testUserId = "test-user-id";
        var board = await db.Boards.Include(b => b.Tags)
            .FirstOrDefaultAsync(b => b.OwnerId == testUserId);
        Assert.NotNull(board); // Make sure the test user has a board
        
        var formData = new Dictionary<string, string>
        {
            {"tagName", "Test Tag"},
            {"tagColour", "#dc3545"} // <- should be RED
        };
        var content = new FormUrlEncodedContent(formData);
        
        // ACT
        var response = await _client.PostAsync($"Board/CreateNewTag?boardId={board.Id}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var returnedTag = await response.Content.ReadFromJsonAsync<Tag>();
        
        // Verify tag properties
        returnedTag.Should().NotBeNull();
        returnedTag.Name.Should().Be("Test Tag");
        returnedTag.TagColourHexadecimal.Should().Be("#dc3545");
    }
}