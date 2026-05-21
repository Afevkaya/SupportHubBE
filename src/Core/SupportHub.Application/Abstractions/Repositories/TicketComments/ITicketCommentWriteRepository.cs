using SupportHub.Domain.Entities;

namespace SupportHub.Application.Abstractions.Repositories.TicketComments;

public interface ITicketCommentWriteRepository
{
    Task<TicketComment> CreateAsync(TicketComment ticketComment);
}