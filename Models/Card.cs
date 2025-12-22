using System.ComponentModel.DataAnnotations;

namespace Taskr.Models;

public class Card
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int Position { get; set; } // Ordering within swimlane
    
    public int SwimlaneId { get; set; }
    
    public Swimlane? Swimlane { get; set; }
}