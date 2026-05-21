using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetOpenTickets;

public record GetOpenTicketsQuery(int Page, int PageSize, string SortBy, string SortDirection) : IQuery<GetOpenTicketsQueryResponse>;