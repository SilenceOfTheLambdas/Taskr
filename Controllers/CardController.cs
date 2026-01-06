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
    public async Task<IActionResult> CreateNewCard(int swimlaneId, [FromForm]string cardTitle, [FromForm]string? cardDescription)
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

    /// <summary>
    /// Deletes a card from the database
    /// </summary>
    /// <param name="cardId">The ID of the card to delete.</param>
    /// <returns>204 - If Successful, NotFound() otherwise.</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteCard([FromQuery]int cardId)
    {
        var card = await dbContext.Cards.FindAsync(cardId);
        if (card == null) return NotFound();
        
        dbContext.Remove(card);
        await dbContext.SaveChangesAsync();
        
        return StatusCode(204);
    }
}