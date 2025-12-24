using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taskr.Services;

namespace Taskr.Controllers;

[Authorize]
public class BoardController(Services.BoardController boardController) : Controller
{
    // GET: Index (Shows Kanban Board)
    public async Task<IActionResult> Index()
    {
        var board = await boardController.GetOrCreateCurrentUserKanbanBoardAsync();
        if (board == null) return Challenge();
        return View(board);
    }
}