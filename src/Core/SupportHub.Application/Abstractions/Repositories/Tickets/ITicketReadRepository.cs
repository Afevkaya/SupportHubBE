using SupportHub.Application.DTOs.Responses;
using SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;
using SupportHub.Application.Features.Tickets.Queries.Tickets.GetOpenTickets;
using SupportHub.Domain.Entities;

namespace SupportHub.Application.Abstractions.Repositories.Tickets;

public interface ITicketReadRepository
{
    Task<GetAllTicketsQueryResponse> GetAllAsync(int page, int pageSize);
    Task<GetOpenTicketsQueryResponse> GetOpenTicketsAsync(int page, int pageSize,
        CancellationToken cancellationToken = default);
    Task<Ticket?> GetTicketDetail(Guid id);
}