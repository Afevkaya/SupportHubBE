using SupportHub.Api.Entities;
using SupportHub.Api.Enums;

namespace SupportHub.Api.Repositories.Tickets;

public interface ITicketRepository
{
    Task<List<Ticket>> GetAllAsync();
    Task<Ticket> CreateAsync(Ticket ticket);
    Task<Ticket> UpdateStatusAsync(Guid id, TicketStatusType status);
}