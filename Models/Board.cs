using System.ComponentModel.DataAnnotations;

namespace Taskr.Models;

public class Board
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ICollection<Swimlane> Swimlanes { get; set; } = new List<Swimlane>();
}