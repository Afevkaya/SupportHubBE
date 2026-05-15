using Microsoft.EntityFrameworkCore;

namespace SupportHub.Api.Contexts;

public class SupportHubDbContext(DbContextOptions<SupportHubDbContext> options) : DbContext(options)
{
    public DbSet<Entities.Ticket> Tickets { get; set; }
}