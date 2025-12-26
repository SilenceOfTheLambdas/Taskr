using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Taskr.Models;
using Taskr.Models.User;

namespace Taskr.Data;

public class KanbanDbContext(DbContextOptions<KanbanDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<Swimlane> Swimlanes => Set<Swimlane>();
    public DbSet<Card> Cards => Set<Card>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Board)
            .WithOne(b => b.Owner)
            .HasForeignKey<Board>(b => b.OwnerId)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Swimlane>()
            .HasOne(s => s.Board)
            .WithMany(b => b.Swimlanes)
            .HasForeignKey(s => s.BoardId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Card>()
            .HasOne(c => c.Swimlane)
            .WithMany(s => s.Cards)
            .HasForeignKey(c => c.SwimlaneId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}