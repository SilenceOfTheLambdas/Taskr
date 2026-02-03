using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Taskr.Models.User;

namespace Taskr.Models;

public class Board
{
    public int Id { get; init; }
    
    [StringLength(120, MinimumLength = 1, ErrorMessage = "Board name must be between 1 and 120 characters long.")]
    public string Title { get; init; } = "Kanban Board";
    public ICollection<Swimlane> Swimlanes { get; init; } = new List<Swimlane>();

    public string? OwnerId { get; init; } = string.Empty;

    public AppUser? Owner { get; init; }
    
    /// <summary>
    /// A collection of tags the user has created that could be applied to cards.
    /// </summary>
    public ICollection<Tag.Tag> Tags { get; set; } = new List<Tag.Tag>();
}