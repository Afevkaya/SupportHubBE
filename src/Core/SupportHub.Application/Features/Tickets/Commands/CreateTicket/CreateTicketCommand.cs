using MediatR;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Features.Tickets.Commands.CreateTicket;

public record CreateTicketCommand(string Title, string Description) : IRequest<ResponseCreateTicket>;