using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Taskr.Models;

namespace Taskr.Data;

public class KanbanDbContext : IdentityDbContext<IdentityUser>
{
    public KanbanDbContext(DbContextOptions<KanbanDbContext> options) : base(options) {}
    
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<Swimlane> Swimlanes => Set<Swimlane>();
    public DbSet<Card> Cards => Set<Card>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
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