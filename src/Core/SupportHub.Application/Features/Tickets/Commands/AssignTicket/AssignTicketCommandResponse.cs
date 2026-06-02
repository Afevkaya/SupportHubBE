namespace SupportHub.Application.Features.Tickets.Commands.AssignTicket;

public record AssignTicketCommandResponse(Guid TicketId, Guid AssignedAgentId, string Message);