namespace SupportHub.Application.Features.Tickets.Commands.CreateTicketComment;

public record CreateTicketCommentCommandResponse(Guid Id, Guid TicketId, string AuthorName, string Message, DateTime CreatedDate);