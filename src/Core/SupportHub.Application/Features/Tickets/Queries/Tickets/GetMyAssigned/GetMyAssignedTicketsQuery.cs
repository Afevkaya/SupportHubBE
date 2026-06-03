using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetMyAssigned;

public record GetMyAssignedTicketsQuery : IQuery<List<GetMyAssignedTicketsQueryResponse>>, ICacheableQuery
{
    string ICacheableQuery.GetCacheKey(Guid? currentUserId) => $"tickets_my_assigned_user_{currentUserId}";

    TimeSpan ICacheableQuery.Expiration =>
        TimeSpan.FromMinutes(2);
}
