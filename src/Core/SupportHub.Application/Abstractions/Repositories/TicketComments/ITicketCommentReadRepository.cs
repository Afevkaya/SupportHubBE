using SupportHub.Domain.Entities;

namespace SupportHub.Application.Abstractions.Repositories.TicketComments;

public interface ITicketCommentReadRepository
{
    Task<List<TicketComment>> GetCommentsByTicketIdAsync(Guid ticketId, CancellationToken cancellationToken = default);
}