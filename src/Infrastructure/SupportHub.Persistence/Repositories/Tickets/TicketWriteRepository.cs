using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Domain.Entities;
using SupportHub.Domain.Enums;

namespace Persistence.Repositories.Tickets;

public class TicketWriteRepository(SupportHubDbContext context) : ITicketWriteRepository
{
    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        var entity = await context.Tickets.AddAsync(ticket);
        return entity.Entity;
    }

    public async Task<Ticket> UpdateStatusAsync(Guid id, TicketStatusType status)
    {
        var ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        if (ticket == null) throw new KeyNotFoundException("Ticket not found.");

        ticket.Status = status;
        ticket.UpdatedDate = DateTime.UtcNow;
        return ticket;
    }
}
