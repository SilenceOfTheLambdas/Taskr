using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taskr.Data;
using Taskr.Models;

namespace Taskr.Controllers;

[Authorize]
public class CardController(Services.BoardController boardController, KanbanDbContext dbContext) : Controller
{
    /// <summary>
    /// Creates a new card in the specified swimlane
    /// </summary>
    /// <param name="swimlaneId">The ID of the swimlane to place the card</param>
    /// <param name="cardTitle">Card Title</param>
    /// <param name="cardDescription">Description of the card</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateNewCard(int swimlaneId, [FromForm]string cardTitle, [FromForm]string cardDescription)
    {
        var board = await boardController.GetOrCreateCurrentUserKanbanBoardAsync();
        if (board == null) return Challenge();
        
        var currentSwimlane = board.Swimlanes.FirstOrDefault(s => s.Id == swimlaneId);
        if (currentSwimlane == null) return BadRequest();

        var newCard = new Card()
        {
            Swimlane = currentSwimlane,
            SwimlaneId = currentSwimlane.Id,
            Title = cardTitle,
            Description = cardDescription,
            Position = currentSwimlane.Cards.LastOrDefault()?.Position ?? 0 + 1
        };
        
        dbContext.Add(newCard);
        await dbContext.SaveChangesAsync();
        
        return StatusCode(201);
    }
}