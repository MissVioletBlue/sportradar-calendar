using Microsoft.EntityFrameworkCore;
using Sportradar.Calendar.Domain.Entities;
using Sportradar.Calendar.Domain.Enums;

namespace Sportradar.Calendar.Infrastructure.Persistence;

// ef core db context gluing together sports and events tables
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        // options come from dependency injection so nothing fancy here
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // when tests forget to pass provider i fallback to default postgres config
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql();
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // important to call base so ef core can wire internal metadata first
        base.OnModelCreating(modelBuilder);

        // configure sport entity rules and seed base sports
        modelBuilder.Entity<Sport>(builder =>
        {
            // sport name is required and limited to 60 characters
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(60);

            // seeding some default sports so UI is not empty on startup
            builder.HasData(
                new Sport(1, "Football"),
                new Sport(2, "Ice Hockey"),
                new Sport(3, "Basketball"));
        });

        // configure event entity rules and seed sample events
        modelBuilder.Entity<EntityEvent>(builder =>
        {
            // title should always exist and never be crazy long
            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);

            // storing status as int keeps database column tiny and simple
            builder.Property(e => e.Status)
                .HasConversion<int>()
                .HasDefaultValue(EventStatus.Scheduled);

            // each event must reference sport but deleting sport should fail
            builder.HasOne<Sport>()
                .WithMany()
                .HasForeignKey(e => e.SportId)
                .OnDelete(DeleteBehavior.Restrict);

            // seed two example events so calendar has something to show
            builder.HasData(
                new EntityEvent(1, 1, new DateTimeOffset(new DateTime(2025, 1, 1, 18, 0, 0, DateTimeKind.Unspecified), TimeSpan.Zero),
                    "Salzburg VS Sturm"),
                new EntityEvent(2, 2, new DateTimeOffset(new DateTime(2025, 1, 2, 19, 30, 0, DateTimeKind.Unspecified), TimeSpan.Zero),
                    "KAC VS Capitals"));
        });
    }

    // db sets so ef core can build queries for events and sports
    public DbSet<EntityEvent> Events { get; set; } = default!;
    public DbSet<Sport> Sports { get; set; } = default!;
}