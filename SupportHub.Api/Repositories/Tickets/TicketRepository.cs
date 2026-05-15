using Microsoft.EntityFrameworkCore;
using SupportHub.Api.Contexts;
using SupportHub.Api.Entities;
using SupportHub.Api.Enums;

namespace SupportHub.Api.Repositories.Tickets;

public class TicketRepository(SupportHubDbContext context) : ITicketRepository
{
    private IQueryable<Ticket> AsQueryable()
    {
        return context.Tickets.AsQueryable().AsNoTracking();
    }

    public async Task<List<Ticket>> GetAllAsync()
    {
        return await AsQueryable().ToListAsync();
    }
    
    public async Task<List<Ticket>> GetOpenTicketsAsync()
    {
        var openTickets = await AsQueryable().Where(t => t.Status == TicketStatusType.Open).ToListAsync();
        return openTickets;
    }

    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        var entity = await context.Tickets.AddAsync(ticket);
        await context.SaveChangesAsync();
        return entity.Entity;
    }

    public async Task<Ticket> UpdateStatusAsync(Guid id, TicketStatusType status)
    {
        var ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null) throw new KeyNotFoundException("Ticket not found.");

        ticket.Status = status;
        ticket.UpdatedDate = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return ticket;
    }
}