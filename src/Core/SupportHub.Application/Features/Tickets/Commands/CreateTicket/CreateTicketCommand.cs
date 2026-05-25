using SupportHub.Application.Abstractions.Messaging;
using SupportHub.Application.DTOs.Responses.Tickets;
using SupportHub.Domain.Enums;

namespace SupportHub.Application.Features.Tickets.Commands.CreateTicket;

public record CreateTicketCommand(string Title, string Description, TicketPriorityType? Priority) : ICommand<ResponseCreateTicket>;