using SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;
using SupportHub.Application.Features.Tickets.Queries.Tickets.GetOpenTickets;
using SupportHub.Domain.Entities;

namespace SupportHub.Application.Abstractions.Repositories.Tickets;

public interface ITicketReadRepository
{
    Task<GetAllTicketsQueryResponse> GetAllAsync(int page, int pageSize, Guid? userId, string sortBy = "CreatedDate",
        string sortDirection = "desc", string? status = null, string priority = "", string search = "");
    Task<GetOpenTicketsQueryResponse> GetOpenTicketsAsync(int page, int pageSize, Guid? userId,
        string sortBy = "CreatedDate", string sortDirection = "desc",
        CancellationToken cancellationToken = default);
    Task<Ticket?> GetTicketAsync(Guid id);
    Task<Ticket?> GetTicketDetail(Guid id);
    Task<List<Ticket>> GetMyAssignedTicketsAsync(Guid agentId, CancellationToken cancellationToken = default);
    Task<bool> AnyTicketAsync(Guid id);
}