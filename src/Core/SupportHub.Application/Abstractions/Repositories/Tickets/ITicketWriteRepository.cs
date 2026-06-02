using SupportHub.Domain.Entities;
using SupportHub.Domain.Enums;

namespace SupportHub.Application.Abstractions.Repositories.Tickets;

public interface ITicketWriteRepository
{
    Task<Ticket> CreateAsync(Ticket ticket);
    Task<Ticket> UpdateStatusAsync(Guid id, TicketStatusType status);
    Task<Ticket> AssignTicketToUserAsync(Guid ticketId, Guid assignedAgentId);
    
}