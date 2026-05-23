using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SupportHub.Domain.Entities;
using SupportHub.Domain.Entities.Identity;

namespace Persistence.Contexts;

public class SupportHubDbContext(DbContextOptions<SupportHubDbContext> options) : IdentityDbContext<AppUser,IdentityRole<Guid>,Guid>(options)
{
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketComment> TicketComments { get; set; }
    public DbSet<TicketActivity> TicketActivities { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>().Property(t => t.Status).HasConversion<string>();
        modelBuilder.Entity<Ticket>().Property(t => t.Priority).HasConversion<string>();
        modelBuilder.Entity<TicketActivity>().Property(t => t.ActivityType).HasConversion<string>();
        modelBuilder.Entity<Ticket>()
            .HasMany(t => t.TicketComments)
            .WithOne(c => c.Ticket)
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Ticket>()
            .HasMany(t => t.TicketActivities)
            .WithOne(a => a.Ticket)
            .HasForeignKey(a => a.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
        base.OnModelCreating(modelBuilder);
    }
}
