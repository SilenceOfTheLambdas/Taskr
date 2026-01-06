using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Taskr.Models.User;

namespace Taskr.Models;

public class Board
{
    public int Id { get; init; }
    public string Title { get; init; } = "Kanban Board";
    public ICollection<Swimlane> Swimlanes { get; init; } = new List<Swimlane>();
    
    public string? OwnerId { get; init; } = string.Empty;
    
    public AppUser? Owner { get; init; }
}