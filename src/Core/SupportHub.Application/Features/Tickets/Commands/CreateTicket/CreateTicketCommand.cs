using MediatR;
using SupportHub.Application.DTOs.Responses;
using SupportHub.Domain.Enums;

namespace SupportHub.Application.Features.Tickets.Commands.CreateTicket;

public record CreateTicketCommand(string Title, string Description, TicketPriorityType? TicketPriorityType) : IRequest<ResponseCreateTicket>;