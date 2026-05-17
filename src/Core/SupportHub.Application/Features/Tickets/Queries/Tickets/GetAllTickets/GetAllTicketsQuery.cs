using MediatR;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;

public record GetAllTicketsQuery() : IRequest<List<ResponseGetTicket>>;