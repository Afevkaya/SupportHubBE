using System.ComponentModel.DataAnnotations.Schema;
using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetOpenTickets;

public record GetOpenTicketsQuery(
    int Page, 
    int PageSize, 
    string SortBy, 
    string SortDirection) : IQuery<GetOpenTicketsQueryResponse>, ICacheableQuery
{
    public string GetCacheKey(Guid? currentUserId) =>
        $"tickets_open_user_{currentUserId}_page_{Page}_size_{PageSize}_sort_{SortBy}_{SortDirection}";

    [NotMapped]
    public TimeSpan Expiration =>
        TimeSpan.FromMinutes(2);
}
