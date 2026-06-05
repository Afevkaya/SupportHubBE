using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetTicketDetail;

public record GetTicketDetailQuery(Guid Id) : IQuery<GetTicketDetailQueryResponse>, ICacheableQuery
{
    string ICacheableQuery.GetCacheKey(Guid? currentUserId) =>
        $"tickets_detail_user_{currentUserId}_ticket_{Id}";

    TimeSpan ICacheableQuery.Expiration =>
        TimeSpan.FromMinutes(2);
}
