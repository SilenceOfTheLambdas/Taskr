using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taskr.Data;
using Taskr.Models;
using Taskr.Services;

namespace Taskr.Controllers;

[Authorize]
public class SwimlaneController(Services.BoardController boardController, KanbanDbContext dbContext) : Controller
{
    // POST: /Swimlane/CreateNewSwimlane
    /// <summary>
    /// Creates a new swimlane for the current user's board
    /// </summary>
    /// <param name="swimlaneName">The name of the new Swimlane</param>
    /// <returns>Status 201 if created.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateNewSwimlane(string swimlaneName)
    {
        var board = await boardController.GetOrCreateCurrentUserKanbanBoardAsync();
        if (board == null) return Challenge();
        var newSwimlane = new Swimlane()
        {
            Board = board,
            BoardId = board.Id,
            Name = swimlaneName,
            Position = board.Swimlanes.LastOrDefault()!.Position + 1
        };
        dbContext.Add(newSwimlane);
        await dbContext.SaveChangesAsync();
        
        return StatusCode(201);
    }

    /// <summary>
    /// Removes a swimlane from the board with a given swimlane ID
    /// </summary>
    /// <param name="swimlaneId">The ID of the swimlane</param>
    /// <returns>204 if successful, A Challenge() if no board is found, and 400 if no swimlane with
    /// a matching swimlaneId is located in the board.</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteSwimlane([FromQuery] int swimlaneId)
    {
        var board = await boardController.GetOrCreateCurrentUserKanbanBoardAsync();
        if (board == null) return Challenge();

        var swimlaneToDelete = board.Swimlanes.FirstOrDefault(s => s.Id == swimlaneId);
        if (swimlaneToDelete == null) return BadRequest($"Could not find swimlane with id: {swimlaneId}");
        
        board.Swimlanes.Remove(swimlaneToDelete);
        await dbContext.SaveChangesAsync();
        
        return StatusCode(204);
    }

    /// <summary>
    /// Performs a HttpPATCH request to update the name of a swimlane with a given swimlane ID
    /// </summary>
    /// <param name="swimlaneId">The ID of the swimlane to update</param>
    /// <param name="newSwimlaneName">The new name of the swimlane</param>
    /// <returns>204 if successful, A Challenge() if no board is found, and 400 if no swimlane with
    /// a matching swimlaneId is located in the board.</returns>
    [HttpPatch]
    public async Task<IActionResult> UpdateSwimlaneTitle([FromQuery] int swimlaneId, [FromQuery] string newSwimlaneName)
    {
        var board = await boardController.GetOrCreateCurrentUserKanbanBoardAsync();
        if (board == null) return Challenge();
        
        var swimlaneToUpdate = board.Swimlanes.FirstOrDefault(s => s.Id == swimlaneId);
        if (swimlaneToUpdate == null) return BadRequest($"Could not find swimlane with id: {swimlaneId}");
        
        swimlaneToUpdate.Name = newSwimlaneName;
        await dbContext.SaveChangesAsync();
        
        return StatusCode(204);
    }
}