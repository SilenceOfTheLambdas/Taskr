using Microsoft.EntityFrameworkCore;

namespace Taskr.Data;

public class KanbanDbContext : DbContext
{
    public KanbanDbContext(DbContextOptions<KanbanDbContext> options) : base(options) {}
}