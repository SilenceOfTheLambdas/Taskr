using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taskr.Data;
using Taskr.Models;
using Taskr.Services;

namespace Taskr.Controllers;

[Authorize]
public class SwimlaneController(BoardService boardService, KanbanDbContext dbContext) : Controller
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
        var board = await boardService.GetOrCreateCurrentUserKanbanBoardAsync();
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
}