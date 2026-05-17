using MediatR;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetOpenTickets;

public record GetOpenTicketsQuery() : IRequest<List<ResponseGetTicket>>;