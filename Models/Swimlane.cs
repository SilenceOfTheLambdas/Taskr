using System.ComponentModel.DataAnnotations;

namespace Taskr.Models;

public class Swimlane
{
    public int Id { get; init; }

    [Required]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Swimlane name must be between 1 and 50 characters long.")]
    public string Name { get; set; } = string.Empty;

    public int Position { get; set; } // Ordering of swimlanes
    public ICollection<Card> Cards { get; set; } = new List<Card>();
    public int BoardId { get; init; }
    public Board? Board { get; init; }
}