using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Taskr.Data;

namespace Taskr.Controllers;

public class BoardController(KanbanDbContext context) : Controller
{
    // GET: Board first board
    public async Task<IActionResult> Index()
    {
        var board = await context.Boards
            .Include(b => b.Swimlanes)
            .ThenInclude(s => s.Cards)
            .FirstOrDefaultAsync();
        if (board == null)
        {
            return NotFound("No boards found. Please create one!");
        }
        return View(board);
    }

    // POST
    [HttpPost]
    public string Create([Bind("BoardName")]string name)
    {
        return $"Created board:{name}";
    }
}