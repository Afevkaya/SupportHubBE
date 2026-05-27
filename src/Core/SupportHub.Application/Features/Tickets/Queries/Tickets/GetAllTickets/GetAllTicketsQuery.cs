using System.ComponentModel.DataAnnotations.Schema;
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
    public string GetCacheKey(Guid? currentUserId) =>
        $"tickets_user_{currentUserId}_page_{Page}_size_{PageSize}_sort_{SortBy}_{SortDirection}";

    [NotMapped]
    public TimeSpan Expiration => TimeSpan.FromMinutes(2);
}
