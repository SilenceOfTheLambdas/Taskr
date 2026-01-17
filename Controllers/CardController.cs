using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taskr.Data;
using Taskr.Models;

namespace Taskr.Controllers;

[Authorize]
public class CardController(Services.BoardService boardService, KanbanDbContext dbContext) : Controller
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
        // Check the model state and make sure it's valid
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var board = await boardService.GetOrCreateCurrentUserKanbanBoardAsync();
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
    /// Updates the title and description of a card with a given card ID. Description can be left blank if desired.
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="cardTitle"></param>
    /// <param name="cardDescription"></param>
    /// <returns>204 if the update is successful, 404 if a card could not be found.</returns>
    [HttpPatch]
    public async Task<IActionResult> UpdateCardDetails([FromQuery]int cardId, [FromForm]string cardTitle, [FromForm]string? cardDescription)
    {
        // Check the model state and make sure it's valid
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var card = await dbContext.Cards.FindAsync(cardId);
        if (card == null) return NotFound();
        
        await UpdateCardName(card, cardTitle);
        await UpdateCardDescription(card, cardDescription ?? "");
        
        return StatusCode(204);
    }

    /// <summary>
    /// Updates the name of a card with a given card ID
    /// </summary>
    /// <param name="card">The card to adjust the name of.</param>
    /// <param name="newCardName">The new name for the card</param>
    /// <returns>204 if the change was successful, 404 if no card with the supplied ID is found.</returns>
    [HttpPatch]
    private async Task<IActionResult> UpdateCardName(Card card, string newCardName)
    {
        card.Title = newCardName;
        await dbContext.SaveChangesAsync();
        
        return StatusCode(204);
    }
    
    /// <summary>
    /// Updates the description of a card with a given card ID
    /// </summary>
    /// <param name="card">The card to adjust the description of.</param>
    /// <param name="newDescription">The new description for the card</param>
    /// <returns>204 if the change was successful, 404 if no card with the supplied ID is found.</returns>
    [HttpPatch]
    private async Task<IActionResult> UpdateCardDescription(Card card, string newDescription = "")
    {
        card.Description = newDescription;
        await dbContext.SaveChangesAsync();
        
        return StatusCode(204);
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