using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Taskr.Models.User;

namespace Taskr.Models;

public class Board
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ICollection<Swimlane> Swimlanes { get; set; } = new List<Swimlane>();
    
    public string? OwnerId { get; set; } = string.Empty;
    
    public AppUser? Owner { get; set; }
}