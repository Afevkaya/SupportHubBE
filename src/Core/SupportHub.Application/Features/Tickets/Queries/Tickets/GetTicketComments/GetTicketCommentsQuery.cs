using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetTicketComments;

public record GetTicketCommentsQuery(Guid TicketId) : IQuery<List<GetTicketCommentsQueryResponse>>, ICacheableQuery
{
    string ICacheableQuery.GetCacheKey(Guid? currentUserId) =>
        $"tickets_comments_user_{currentUserId}_ticket_{TicketId}";

    TimeSpan ICacheableQuery.Expiration =>
        TimeSpan.FromMinutes(2);
}
