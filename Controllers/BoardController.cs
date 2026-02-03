using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taskr.Data;
using Taskr.Models.Tag;
using Taskr.Services;

namespace Taskr.Controllers;

[Authorize]
public class BoardController(BoardService boardService, KanbanDbContext dbContext) : Controller
{
    // GET: Index (Shows Kanban Board)
    public async Task<IActionResult> Index()
    {
        var board = await boardService.GetOrCreateCurrentUserKanbanBoardAsync();
        if (board == null) return Challenge();
        return View(board);
    }

    /// <summary>
    /// Creates a new tag associated with a specific board.
    /// </summary>
    /// <param name="boardId">The ID of the board to which the tag will be added.</param>
    /// <param name="tagName">The name of the tag being created. Must be between 1 and 20 characters long.</param>
    /// <param name="tagColour">
    /// An optional hexadecimal colour value representing the tag's colour.
    /// Defaults to white ("#ffffff") if no value is provided.
    /// </param>
    /// <returns>
    /// Returns a 201 Created status code along with the newly created tag if successful.
    /// Returns a 404 Not Found status code if the board with the specified ID does not exist.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> CreateNewTag(int boardId, [FromForm]string tagName, [FromForm]string? tagColour)
    {
        var board = await dbContext.Boards.Include(b => b.Tags)
            .FirstOrDefaultAsync(b => b.Id == boardId);
        if (board == null) return NotFound();

        var newTag = new Tag
        {
            Name = tagName,
            TagColourHexadecimal = tagColour ?? Colours.White
        };
        board.Tags.Add(newTag);
        await dbContext.SaveChangesAsync();

        return Created("/Board/CreateNewTag", newTag);
    }
}