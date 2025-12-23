using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taskr.Data;
using Taskr.Services;

namespace Taskr.Controllers;

[Authorize]
public class BoardController : Controller
{
    private readonly BoardService _boardService;
    private readonly KanbanDbContext _context;

    public BoardController(BoardService boardService, KanbanDbContext context)
    {
        _boardService = boardService;
        _context = context;
    }
    
    // GET: Index (Shows Kanban Board)
    public async Task<IActionResult> Index()
    {
        var board = await _boardService.GetOrCreateCurrentUserKanbanBoardAsync();
        if (board == null) return Challenge();
        return View(board);
    }

    // POST
    [HttpPost]
    public string Create([Bind("BoardName")]string name)
    {
        return $"Created board:{name}";
    }
}