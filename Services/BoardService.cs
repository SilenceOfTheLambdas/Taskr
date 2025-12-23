using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Taskr.Data;
using Taskr.Models;
using Taskr.Models.User;

namespace Taskr.Services;

public class BoardService
{
    private readonly KanbanDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BoardService(KanbanDbContext dbContext, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets or creates a board for the current user
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if no user is authenticated.</exception>
    public async Task<Board?> GetOrCreateCurrentUserKanbanBoardAsync()
    {
        // Guard against a missing HttpContext (should never happen in MVC,
        // but it protects background jobs that might reuse the service)
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        // GetUserAsync will return null if the principal has no NameIdentifier claim
        var user = await _userManager.GetUserAsync(httpContext.User);
        if (user == null) return null;   // unauthenticated – caller decides what to do

        // Check to see if the current user has a board
        var board = await _dbContext.Boards
            .Include(b => b.Swimlanes)
            .ThenInclude(s => s.Cards)
            .FirstOrDefaultAsync(b => b.OwnerId == user.Id);

        // If they don't, create one
        if (board == null)
        {
            board = new Board()
            {
                Title = $"{user.UserName}'s Board",
                OwnerId = user.Id
            };
            _dbContext.Add(board);
            await _dbContext.SaveChangesAsync();
            
            // Now create the default swimlanes for the board
            var swimlanes = CreateNewUserSwimlanes(board.Id);

            foreach (var swimlane in swimlanes)
            {
                _dbContext.Add(swimlane);
            }
            await _dbContext.SaveChangesAsync();
        }
        
        return board;
    }
    
    private List<Swimlane> CreateNewUserSwimlanes(int boardId)
    {
        var backlogSwimlane = new Swimlane()
        {
            Name = "Backlog",
            BoardId = boardId,
            Position = 0
        };
        var todoSwimlane = new Swimlane()
        {
            Name = "To Do",
            BoardId = boardId,
            Position = 1
        };
        var completedSwimlane = new Swimlane()
        {
            Name = "Completed",
            BoardId = boardId,
            Position = 2
        };
        
        return [backlogSwimlane, todoSwimlane, completedSwimlane];
    }
}