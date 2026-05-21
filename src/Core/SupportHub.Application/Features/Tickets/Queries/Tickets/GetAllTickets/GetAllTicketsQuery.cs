using MediatR;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;

public record GetAllTicketsQuery(int Page, int PageSize, string? Status, string Search, string Priority, string SortBy, string SortDirection) : IRequest<GetAllTicketsQueryResponse>;