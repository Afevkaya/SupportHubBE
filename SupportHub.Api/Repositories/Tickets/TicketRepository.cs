using SupportHub.Api.Entities;
using SupportHub.Api.Enums;

namespace SupportHub.Api.Repositories.Tickets;

public class TicketRepository : ITicketRepository
{
    private static readonly List<Ticket> Tickets = [];
    public Task<List<Ticket>> GetAllAsync()
    {
        return Task.FromResult(Tickets);
    }
    public Task<Ticket> CreateAsync(Ticket ticket)
    {
        Tickets.Add(ticket);
        return Task.FromResult(ticket);
    }
    public Task<Ticket> UpdateStatusAsync(Guid id, TicketStatusType status)
    {
        var ticket = Tickets.FirstOrDefault(t => t.Id == id);
        if (ticket == null) throw new KeyNotFoundException("Ticket not found.");

        ticket.Status = nameof(status);
        ticket.UpdatedDate = DateTime.UtcNow;
        return Task.FromResult(ticket);
    }
}