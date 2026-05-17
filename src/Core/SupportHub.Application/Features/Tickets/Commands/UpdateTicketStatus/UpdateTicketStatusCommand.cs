using MediatR;
using SupportHub.Application.DTOs.Responses;
using SupportHub.Domain.Enums;

namespace SupportHub.Application.Features.Tickets.Commands.UpdateTicketStatus;

public record UpdateTicketStatusCommand(Guid Id, TicketStatusType Status) : IRequest<ResponseUpdateTicketStatus>;