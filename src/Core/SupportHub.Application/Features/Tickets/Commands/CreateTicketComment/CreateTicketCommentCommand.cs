using MediatR;

namespace SupportHub.Application.Features.Tickets.Commands.CreateTicketComment;

public record CreateTicketCommentCommand(Guid TicketId, string AuthorName, string Message) : IRequest<CreateTicketCommentCommandResponse>;