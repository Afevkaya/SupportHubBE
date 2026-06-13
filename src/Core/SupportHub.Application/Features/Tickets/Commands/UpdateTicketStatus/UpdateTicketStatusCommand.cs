using System.Text.Json.Serialization;
using SupportHub.Application.Abstractions.Messaging;
using SupportHub.Application.DTOs.Responses;
using SupportHub.Application.DTOs.Responses.Tickets;
using SupportHub.Domain.Enums;

namespace SupportHub.Application.Features.Tickets.Commands.UpdateTicketStatus;

public record UpdateTicketStatusCommand(
    [property: JsonIgnore] Guid Id, 
    TicketStatusType Status) : ICommand<ResponseUpdateTicketStatus>;