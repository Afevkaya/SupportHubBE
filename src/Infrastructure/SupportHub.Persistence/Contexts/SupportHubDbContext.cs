using Microsoft.EntityFrameworkCore;
using SupportHub.Domain.Entities;

namespace Persistence.Contexts;

public class SupportHubDbContext(DbContextOptions<SupportHubDbContext> options) : DbContext(options)
{
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketComment> TicketComments { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>().Property(t => t.Status).HasConversion<string>();
        modelBuilder.Entity<Ticket>().Property(t => t.Priority).HasConversion<string>();
        modelBuilder.Entity<Ticket>()
            .HasMany(t => t.TicketComments)
            .WithOne(c => c.Ticket)
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
        base.OnModelCreating(modelBuilder);
    }
}
