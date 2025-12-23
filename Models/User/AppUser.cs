using Microsoft.AspNetCore.Identity;

namespace Taskr.Models.User;

public class AppUser : IdentityUser
{
    public Board? Board { get; set; } // Set a 1:1 relationship with Board
}