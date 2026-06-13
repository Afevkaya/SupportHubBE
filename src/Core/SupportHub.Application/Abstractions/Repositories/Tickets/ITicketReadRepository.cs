using SupportHub.Application.Features.Tickets.Queries.GetAllTickets;
using SupportHub.Domain.Entities;

namespace SupportHub.Application.Abstractions.Repositories.Tickets;

public interface ITicketReadRepository
{
    Task<GetAllTicketsQueryResponse> GetAllAsync(int page,
        int pageSize,
        Guid? createdByUserId,
        Guid? assignedAgentId,
        string? sortBy = "createdDate",
        string? sortDirection = "desc",
        string? status = null,
        string? priority = null,
        string? search = null);
    Task<Ticket?> GetTicketAsync(Guid id);
    Task<Ticket?> GetTicketDetail(Guid id, Guid? createdByUserId, Guid? assignedAgentId);
}
