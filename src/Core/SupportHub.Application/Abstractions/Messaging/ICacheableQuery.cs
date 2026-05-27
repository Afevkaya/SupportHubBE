using System.ComponentModel.DataAnnotations.Schema;

namespace SupportHub.Application.Abstractions.Messaging;

public interface ICacheableQuery
{
    string GetCacheKey(Guid? currentUserId);
    [NotMapped]
    TimeSpan Expiration { get; }
}
