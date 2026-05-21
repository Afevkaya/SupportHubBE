using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Tickets.Commands.CreateTicketComment;

public record CreateTicketCommentCommand(Guid TicketId, string AuthorName, string Message) : ICommand<CreateTicketCommentCommandResponse>;