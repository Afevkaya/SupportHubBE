using SupportHub.Application.DTOs.Responses.Auths;

namespace SupportHub.Application.Features.Tickets.Commands.CreateTicketComment;

public record CreateTicketCommentCommandResponse(
    Guid Id, 
    Guid TicketId, 
    string Message, 
    DateTime CreatedDate,
    ResponseGetAuth? Author);