using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Taskr.Data;
using Taskr.Models;
using Taskr.Models.User;

namespace Taskr.Services;

public class BoardService(
    KanbanDbContext dbContext,
    UserManager<AppUser> userManager,
    IHttpContextAccessor httpContextAccessor)
{
    /// <summary>
    /// Gets or creates a board for the current user
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if no user is authenticated.</exception>
    public async Task<Board?> GetOrCreateCurrentUserKanbanBoardAsync()
    {
        // Guard against a missing HttpContext (should never happen in MVC,
        // but it protects background jobs that might reuse the service)
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        // GetUserAsync will return null if the principal has no NameIdentifier claim
        var user = await userManager.GetUserAsync(httpContext.User);
        if (user == null) return null; // unauthenticated – caller decides what to do

        // Check to see if the current user has a board
        var board = await dbContext.Boards
            .Include(b => b.Swimlanes)
            .ThenInclude(s => s.Cards)
            .FirstOrDefaultAsync(b => b.OwnerId == user.Id);

        // If they don't, create one
        if (board == null)
        {
            board = new Board
            {
                Title = $"{user.UserName}'s Board",
                OwnerId = user.Id
            };
            dbContext.Add(board);
            await dbContext.SaveChangesAsync();

            // Now create the default swimlanes for the board
            var swimlanes = CreateNewUserSwimlanes(board.Id);
            
            // Create example cards
            var cards = CreateNewUserCards(swimlanes.First().Id);

            foreach (var swimlane in swimlanes) dbContext.Add(swimlane);
            await dbContext.SaveChangesAsync();

            foreach (var card in cards)
            {
                board.Swimlanes.First().Cards.Add(card);
                dbContext.Add(card);
            }
            await dbContext.SaveChangesAsync();
        }

        return board;
    }

    private List<Swimlane> CreateNewUserSwimlanes(int boardId)
    {
        var backlogSwimlane = new Swimlane
        {
            Name = "Backlog",
            BoardId = boardId,
            Position = 1
        };
        var todoSwimlane = new Swimlane
        {
            Name = "To Do",
            BoardId = boardId,
            Position = 2
        };
        var completedSwimlane = new Swimlane
        {
            Name = "Completed",
            BoardId = boardId,
            Position = 3
        };

        return [backlogSwimlane, todoSwimlane, completedSwimlane];
    }

    private List<Card> CreateNewUserCards(int swimlaneId)
    {
        var card1 = new Card
        {
            CreatedAt = DateTime.UtcNow.Date.Date,
            Description =
                "This is a card, you can drag me around and move me to different swimlanes or adjust the order.",
            Id = 1,
            Position = 1,
            SwimlaneId = swimlaneId,
            Title = "Welcome to Taskr!"
        };
        var card2 = new Card
        {
            CreatedAt = DateTime.UtcNow.Date.Date,
            Description =
                "This is a card, you can drag me around and move me to different swimlanes or adjust the order.",
            Id = 2,
            Position = 2,
            SwimlaneId = swimlaneId,
            Title = "Welcome to Taskr!"
        };
        
        return [card1, card2];
    }
}