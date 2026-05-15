using Microsoft.EntityFrameworkCore;

namespace SupportHub.Api.Contexts;

public class SupportHubDbContext(DbContextOptions<SupportHubDbContext> options) : DbContext(options)
{
    public DbSet<Entities.Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entities.Ticket>().Property(t => t.Status).HasConversion<string>();
        base.OnModelCreating(modelBuilder);
    }
}