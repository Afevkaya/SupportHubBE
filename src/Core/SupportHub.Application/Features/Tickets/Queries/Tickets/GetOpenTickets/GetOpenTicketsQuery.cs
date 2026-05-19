using MediatR;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetOpenTickets;

public record GetOpenTicketsQuery(int Page, int PageSize, string SortBy, string SortDirection) : IRequest<GetOpenTicketsQueryResponse>;