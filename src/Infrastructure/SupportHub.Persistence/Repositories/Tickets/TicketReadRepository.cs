using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Domain.Entities;
using SupportHub.Domain.Enums;

namespace Persistence.Repositories.Tickets;

public class TicketReadRepository(SupportHubDbContext context) : ITicketReadRepository
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
}