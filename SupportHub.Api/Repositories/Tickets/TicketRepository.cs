using SupportHub.Api.Entities;

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
}