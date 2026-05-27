using System.Text.Json.Serialization;
using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;

public record GetAllTicketsQuery(
    int Page,
    int PageSize,
    string? Status,
    string Search,
    string Priority,
    string SortBy,
    string SortDirection)
    : IQuery<GetAllTicketsQueryResponse>, ICacheableQuery
{
    string ICacheableQuery.GetCacheKey(Guid? currentUserId) =>
        $"tickets_user_{currentUserId}_page_{Page}_size_{PageSize}_status_{Status}_search_{Search}_priority_{Priority}_sort_{SortBy}_{SortDirection}";

    TimeSpan ICacheableQuery.Expiration =>
        TimeSpan.FromMinutes(2);
}
