using Microsoft.EntityFrameworkCore;
using SupportHub.Domain.Entities;

namespace Persistence.Contexts;

public class SupportHubDbContext(DbContextOptions<SupportHubDbContext> options) : DbContext(options)
{
    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>().Property(t => t.Status).HasConversion<string>();
        base.OnModelCreating(modelBuilder);
    }
}