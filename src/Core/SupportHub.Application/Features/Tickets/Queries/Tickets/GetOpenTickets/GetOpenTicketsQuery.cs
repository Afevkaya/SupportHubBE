using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetOpenTickets;

public record GetOpenTicketsQuery(
    int Page,
    int PageSize,
    string SortBy,
    string SortDirection)
    : IQuery<GetOpenTicketsQueryResponse>, ICacheableQuery
{
    string ICacheableQuery.GetCacheKey(Guid? currentUserId) =>
        $"tickets_open_user_{currentUserId}_page_{Page}_size_{PageSize}_sort_{SortBy}_{SortDirection}";

    TimeSpan ICacheableQuery.Expiration =>
        TimeSpan.FromMinutes(2);
}
