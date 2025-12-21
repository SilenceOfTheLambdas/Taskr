namespace Taskr.Models;

public class Swimlane
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; } // Ordering of swimlanes
}