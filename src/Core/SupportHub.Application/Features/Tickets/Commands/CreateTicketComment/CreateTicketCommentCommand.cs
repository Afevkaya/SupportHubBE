using System.Text.Json.Serialization;
using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Tickets.Commands.CreateTicketComment;

public record CreateTicketCommentCommand(
    [property: JsonIgnore] Guid TicketId, 
    string Message) : ICommand<CreateTicketCommentCommandResponse>;