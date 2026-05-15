using SupportHub.Api.Entities;

namespace SupportHub.Api.Repositories.Tickets;

public interface ITicketRepository
{
    Task<List<Ticket>> GetAllAsync();
    Task<Ticket> CreateAsync(Ticket ticket);
}