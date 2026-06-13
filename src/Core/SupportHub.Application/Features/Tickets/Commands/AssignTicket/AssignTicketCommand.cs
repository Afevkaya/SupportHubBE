using System.Text.Json.Serialization;
using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Tickets.Commands.AssignTicket;

public record AssignTicketCommand(
    [property: JsonIgnore] Guid TicketId, 
    Guid AssignedAgentId) : ICommand<AssignTicketCommandResponse>;