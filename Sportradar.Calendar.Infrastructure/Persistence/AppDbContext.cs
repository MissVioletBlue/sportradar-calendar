using Microsoft.EntityFrameworkCore;
using Sportradar.Calendar.Domain.Entities;
using Sportradar.Calendar.Domain.Enums;

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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Sport>(builder =>
        {
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(60);

            builder.HasData(
                new Sport(1, "Football"),
                new Sport(2, "Ice Hockey"),
                new Sport(3, "Basketball"));
        });

        modelBuilder.Entity<EntityEvent>(builder =>
        {
            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Status)
                .HasConversion<int>()
                .HasDefaultValue(EventStatus.Scheduled);

            builder.HasOne<Sport>()
                .WithMany()
                .HasForeignKey(e => e.SportId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new EntityEvent(1, 1, new DateTimeOffset(new DateTime(2025, 1, 1, 18, 0, 0, DateTimeKind.Unspecified), TimeSpan.Zero),
                    "Salzburg VS Sturm"),
                new EntityEvent(2, 2, new DateTimeOffset(new DateTime(2025, 1, 2, 19, 30, 0, DateTimeKind.Unspecified), TimeSpan.Zero),
                    "KAC VS Capitals"));
        });
    }

    public DbSet<EntityEvent> Events { get; set; } = default!;
    public DbSet<Sport> Sports { get; set; } = default!;
}