using SupportHub.Domain.Entities;

namespace SupportHub.Application.Abstractions.Repositories.Tickets;

public interface ITicketReadRepository
{
    Task<List<Ticket>> GetAllAsync();
    Task<List<Ticket>> GetOpenTicketsAsync();
}