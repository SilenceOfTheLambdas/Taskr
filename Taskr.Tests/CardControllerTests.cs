using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Taskr.Data;
using Taskr.Models;
using Taskr.Tests.Helper;

namespace Taskr.Tests;

public class CardControllerTests(TaskrWebApplicationFactory factory) : IClassFixture<TaskrWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false
    });
    
    [Fact(DisplayName = "Create New Card")]
    public async Task CreateNewCard_ReturnsSuccess_AndSaveToDb()
    {
        // Arrange
        // Seed the user once
        await factory.SeedTestDataAsync();

        using var scope = factory.Services.CreateScope();
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

    [Fact(DisplayName = "Move Card")]
    public async Task MoveCard_UpdatesPositionAndSwimlane()
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

        var s1 = board!.Swimlanes.First();
        var s2 = board.Swimlanes.Last();

        var card1 = new Card { Title = "Card 1", SwimlaneId = s1.Id, Position = 1 };
        var card2 = new Card { Title = "Card 2", SwimlaneId = s1.Id, Position = 2 };
        var card3 = new Card { Title = "Card 3", SwimlaneId = s2.Id, Position = 1 };
        db.Cards.AddRange(card1, card2, card3);
        await db.SaveChangesAsync();

        // Act - Move card 1 from s1 to s2 at position 0
        var response = await _client.PatchAsync($"Card/MoveCard?cardId={card1.Id}&newSwimlaneId={s2.Id}&newPosition=0", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Reload data by creating a new scope to avoid cache
        using var verifyScope = factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<KanbanDbContext>();

        var updatedCard1 = await verifyDb.Cards.FindAsync(card1.Id);
        var updatedCard2 = await verifyDb.Cards.FindAsync(card2.Id);
        var updatedCard3 = await verifyDb.Cards.FindAsync(card3.Id);

        updatedCard1!.SwimlaneId.Should().Be(s2.Id);
        updatedCard1.Position.Should().Be(1);

        updatedCard3!.SwimlaneId.Should().Be(s2.Id);
        updatedCard3.Position.Should().Be(2); // card 3 should have shifted

        updatedCard2!.SwimlaneId.Should().Be(s1.Id);
        updatedCard2.Position.Should().Be(1); // card 2 should have shifted up in s1
    }
}