using SupportHub.Application.DTOs.Responses;
using SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;
using SupportHub.Domain.Entities;

namespace SupportHub.Application.Abstractions.Repositories.Tickets;

public interface ITicketReadRepository
{
    Task<List<ResponseGetTicket>> GetAllAsync();
    Task<List<ResponseGetTicket>> GetOpenTicketsAsync();
    Task<Ticket?> GetTicketDetail(Guid id);
}