using SupportHub.Domain.Entities;
using SupportHub.Domain.Enums;

namespace SupportHub.Application.Abstractions.Repositories.Tickets;

public interface ITicketRepository
{
    Task<List<Ticket>> GetAllAsync();
    Task<List<Ticket>> GetOpenTicketsAsync();
    Task<Ticket> CreateAsync(Ticket ticket);
    Task<Ticket> UpdateStatusAsync(Guid id, TicketStatusType status);
}