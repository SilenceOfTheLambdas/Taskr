using System.ComponentModel.DataAnnotations;

namespace Taskr.Models.Tag;

public class Tag
{
    [Required]
    public int Id { get; init; }
    
    [Required]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "Tag name must be between 1 and 20 characters long.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(7, ErrorMessage = "Tag colour must be a hexadecimal value of length 7.")]
    public string TagColourHexadecimal { get; set; } = Colours.White;
}