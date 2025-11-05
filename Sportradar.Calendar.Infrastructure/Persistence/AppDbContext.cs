using Microsoft.EntityFrameworkCore;
using Sportradar.Calendar.Domain.Entities;

namespace Sportradar.Calendar.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql();
        }
    }
    
    public DbSet<EntityEvent> Events { get; set; }
}