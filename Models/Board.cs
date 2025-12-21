using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Taskr.Models;

public class Board
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public ICollection<Column> Columns { get; set; } = new List<Column>();
}