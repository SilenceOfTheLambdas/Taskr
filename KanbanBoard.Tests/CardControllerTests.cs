using Taskr.Controllers;
using Taskr.Models;
using Xunit;

namespace Taskr.KanbanBoard.Tests;

public class CardControllerTests
{
    [Fact(DisplayName = "Test Card Creation")]
    public async Task TestCardIsCreated()
    {
        #region Setting-up Enviroment

        TestsHelper.SetupEnvironment(out var fakeUser, out var userManager, out var httpContextAccessor, 
            out var db, out var boardService);
        
        var board = await boardService.GetOrCreateCurrentUserKanbanBoardAsync();
        
        // Make sure we actually got a board back
        Assert.NotNull(board);
        
        var newSwimlane = new Swimlane()
        {
            Board = board,
            BoardId = board.Id,
            Name = "Test Swimlane",
            Position = board.Swimlanes.LastOrDefault()!.Position + 1
        };
        db.Add(newSwimlane);
        await db.SaveChangesAsync();

        #endregion
        
        // ---------- Act ----------
        var newCard = new Card()
        {
            Swimlane = newSwimlane,
            SwimlaneId = newSwimlane.Id,
            Title = "Test Card",
            Description = "This is a test card",
            Position = 0
        };
        db.Add(newCard);
        await db.SaveChangesAsync();
        
        // Test we have just a single card in the swimlane
        Assert.Single(newSwimlane.Cards);
        
        // Test the card title is correct
        Assert.Equal("Test Card", newSwimlane.Cards.First().Title);
        
        // Test the card description is correct
        Assert.Equal("This is a test card", newSwimlane.Cards.First().Description);
        
        // Test that the card belongs to the swimlane
        Assert.Equal(newSwimlane.Id, newCard.SwimlaneId);
    }

    [Fact(DisplayName = "Test Card Deletion")]
    public async Task TestCardIsDeleted()
    {
        #region Setting-up Enviroment

        TestsHelper.SetupEnvironment(out var fakeUser, out var userManager, out var httpContextAccessor, 
            out var db, out var boardService);
        
        var board = await boardService.GetOrCreateCurrentUserKanbanBoardAsync();
        
        // Make sure we actually got a board back
        Assert.NotNull(board);
        
        var newSwimlane = new Swimlane()
        {
            Board = board,
            BoardId = board.Id,
            Name = "Test Swimlane",
            Position = board.Swimlanes.LastOrDefault()!.Position + 1
        };
        db.Add(newSwimlane);
        await db.SaveChangesAsync();

        #endregion
        
        // ---------- Act ----------
        var newCard = new Card()
        {
            Swimlane = newSwimlane,
            SwimlaneId = newSwimlane.Id,
            Title = "Test Card",
            Description = "This is a test card",
            Position = 0
        };
        db.Add(newCard);
        await db.SaveChangesAsync();
        
        db.Remove(newCard);
        await db.SaveChangesAsync();
        
        // Test that the swimlane is now empty
        Assert.Empty(newSwimlane.Cards);
    }

    [Fact(DisplayName = "Test Updating Card Title")]
    public async Task TestCardTitleIsUpdated()
    {
        #region Setting-up Enviroment
        
        TestsHelper.SetupEnvironment(out var fakeUser, out var userManager, out var httpContextAccessor, 
            out var db, out var boardService);
        
        var board = await boardService.GetOrCreateCurrentUserKanbanBoardAsync();
        // Make sure we actually got a board back
        Assert.NotNull(board);
        
        var newSwimlane = new Swimlane()
        {
            Board = board,
            BoardId = board.Id,
            Name = "Test Swimlane",
            Position = board.Swimlanes.LastOrDefault()!.Position + 1
        };
        db.Add(newSwimlane);
        await db.SaveChangesAsync();
        
        var newCard = new Card()
        {
            Swimlane = newSwimlane,
            SwimlaneId = newSwimlane.Id,
            Title = "Test Card",
            Description = "This is a test card",
            Position = 0
        };
        db.Add(newCard);
        await db.SaveChangesAsync();
        
        #endregion
    }
}