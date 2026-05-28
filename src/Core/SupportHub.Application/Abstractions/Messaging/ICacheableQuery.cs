namespace SupportHub.Application.Abstractions.Messaging;

public interface ICacheableQuery
{
    string GetCacheKey(Guid? currentUserId);
    
    TimeSpan Expiration { get; }
}
