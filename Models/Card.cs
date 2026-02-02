using System.ComponentModel.DataAnnotations;

namespace Taskr.Models;

public class Card
{
    public int Id { get; init; }

    [Required]
    [StringLength(40, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 40 characters long.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description character count must not exceed 2000.")]
    public string? Description { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public int Position { get; set; } // Ordering within swimlane

    public int SwimlaneId { get; set; }

    public Swimlane? Swimlane { get; set; }
}