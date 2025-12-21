namespace Taskr.Models;

public class Swimlane
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; } // Ordering of swimlanes
    public ICollection<Card> Cards { get; set; } = new List<Card>();
    public int BoardId { get; set; }
    public Board? Board { get; set; }
}