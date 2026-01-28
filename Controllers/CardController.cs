using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<IActionResult> CreateNewCard(int swimlaneId, [FromForm] string cardTitle,
        [FromForm] string? cardDescription)
    {
        // Check the model state and make sure it's valid
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var board = await boardService.GetOrCreateCurrentUserKanbanBoardAsync();
        if (board == null) return Challenge();

        var currentSwimlane = board.Swimlanes.FirstOrDefault(s => s.Id == swimlaneId);
        if (currentSwimlane == null) return BadRequest();

        var newCard = new Card
        {
            Swimlane = currentSwimlane,
            SwimlaneId = currentSwimlane.Id,
            Title = cardTitle,
            Description = cardDescription,
            Position = (currentSwimlane.Cards.MaxBy(c => c.Position)?.Position ?? 0) + 1
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
    public async Task<IActionResult> UpdateCardDetails([FromQuery] int cardId, [FromForm] string cardTitle,
        [FromForm] string? cardDescription)
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
    /// Moves a card to a new swimlane and/or a new position within a swimlane.
    /// </summary>
    /// <param name="cardId">The ID of the card to move.</param>
    /// <param name="newSwimlaneId">The ID of the destination swimlane.</param>
    /// <param name="newPosition">The new 0-indexed position within the swimlane.</param>
    /// <returns>204 No Content if successful, or error codes.</returns>
    [HttpPatch]
    public async Task<IActionResult> MoveCard([FromQuery] int cardId, [FromQuery] int newSwimlaneId,
        [FromQuery] int newPosition)
    {
        var board = await boardService.GetOrCreateCurrentUserKanbanBoardAsync();
        if (board == null) return Challenge();

        var card = await dbContext.Cards.FindAsync(cardId);
        if (card == null) return NotFound();

        if (board.Swimlanes.All(s => s.Id != card.SwimlaneId)
            || board.Swimlanes.All(s => s.Id != newSwimlaneId))
            return BadRequest();

        var oldSwimlaneId = card.SwimlaneId;

        var allCardsInAffectedSwimlanes = await dbContext.Cards
            .Where(c => c.SwimlaneId == oldSwimlaneId || c.SwimlaneId == newSwimlaneId)
            .ToListAsync();

        // If a card has been moved within the same swimlane
        if (oldSwimlaneId == newSwimlaneId)
        {
            var cards = allCardsInAffectedSwimlanes.OrderBy(c => c.Position).ToList();
            cards.RemoveAll(c => c.Id == card.Id);
            cards.Insert(Math.Clamp(newPosition, 0, cards.Count), card);
            for (var i = 0; i < cards.Count; i++) cards[i].Position = i + 1;
        }
        else // If a card has been moved to a different swimlane
        {
            // Source Swimlane: It removes the card from the source swimlane's list and re-indexes the
            // remaining cards so their positions are sequential (e.g., if you remove card #2, card #3 becomes #2).
            var oldCards = allCardsInAffectedSwimlanes.Where(c => c.SwimlaneId == oldSwimlaneId)
                .OrderBy(c => c.Position).ToList();
            oldCards.RemoveAll(c => c.Id == card.Id);
            for (var i = 0; i < oldCards.Count; i++) oldCards[i].Position = i + 1;

            // Destination Swimlane: It updates the card's SwimlaneId to the new ID, inserts it at the specified
            // newPosition, and then re-indexes all cards in the destination swimlane to accommodate the new arrival.
            var newCards = allCardsInAffectedSwimlanes.Where(c => c.SwimlaneId == newSwimlaneId)
                .OrderBy(c => c.Position).ToList();
            card.SwimlaneId = newSwimlaneId;
            newCards.Insert(Math.Clamp(newPosition, 0, newCards.Count), card);
            for (var i = 0; i < newCards.Count; i++) newCards[i].Position = i + 1;
        }

        await dbContext.SaveChangesAsync();
        return StatusCode(204);
    }

    /// <summary>
    /// Deletes a card from the database
    /// </summary>
    /// <param name="cardId">The ID of the card to delete.</param>
    /// <returns>204 - If Successful, NotFound() otherwise.</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteCard([FromQuery] int cardId)
    {
        var card = await dbContext.Cards.FindAsync(cardId);
        if (card == null) return NotFound();

        dbContext.Remove(card);
        await dbContext.SaveChangesAsync();

        return StatusCode(204);
    }
}