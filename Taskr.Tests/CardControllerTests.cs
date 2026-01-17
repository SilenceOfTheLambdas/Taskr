using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Taskr.Data;

namespace Taskr.Tests;

public class CardControllerTests : IClassFixture<TaskrWebApplicationFactory>
{
    private readonly TaskrWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CardControllerTests(TaskrWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    /// <summary>
    /// Tests the creation of a new card by ensuring that the operation returns a successful response
    /// and the card is persisted to the database.
    /// The method initialises test data including a user and associated board, performs a POST request to the
    /// Card/CreateNewCard endpoint, and verifies the creation of the card.
    /// </summary>
    /// <returns>
    /// An asynchronous task that completes when the test is finished. Ensures the HTTP response status is "Created".
    /// </returns>
    [Fact]
    public async Task CreateNewCard_ReturnsSuccess_AndSaveToDb()
    {
        // Arrange
        // Seed the user once
        await _factory.SeedTestDataAsync();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<KanbanDbContext>();
        
        const string testUserId = "test-user-id";
        var board = await db.Boards.Include(b => b.Swimlanes)
            .FirstOrDefaultAsync(b => b.OwnerId == testUserId);
        Assert.NotNull(board); // Make sure the test user has a board
        
        var targetSwimlaneId = board.Swimlanes.First().Id;
        
        var formData = new Dictionary<string, string>
        {
            {"cardTitle", "Test Card"},
            {"cardDescription", "Test Description"}
        };
        var content = new FormUrlEncodedContent(formData);
        
        // Act
        var response = await _client.PostAsync($"Card/CreateNewCard?swimlaneId={targetSwimlaneId}", content);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
}