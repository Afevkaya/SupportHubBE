using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Tickets.Queries.GetAllTickets;

public record GetAllTicketsQuery(
    int Page = 1,
    int PageSize = 10,
    string? Status = null,
    string? Search = null,
    string? Priority = null,
    string? SortBy = "createdDate",
    string? SortDirection = "desc")
    : IQuery<GetAllTicketsQueryResponse>, ICacheableQuery
{
    string ICacheableQuery.GetCacheKey(Guid? currentUserId) =>
        $"tickets_all_user_{currentUserId}_page_{NormalizePage(Page)}_size_{NormalizePageSize(PageSize)}_status_{Normalize(Status)}_search_{Normalize(Search)}_priority_{Normalize(Priority)}_sort_{Normalize(SortBy)}_{NormalizeSortDirection(SortDirection)}";

    TimeSpan ICacheableQuery.Expiration =>
        TimeSpan.FromMinutes(2);

    private static int NormalizePage(int page) =>
        page <= 0 ? 1 : page;

    private static int NormalizePageSize(int pageSize) =>
        pageSize <= 0 ? 10 : pageSize;

    private static string Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "none" : value.Trim().ToLowerInvariant();

    private static string NormalizeSortDirection(string? sortDirection) =>
        sortDirection?.Trim().Equals("asc", StringComparison.OrdinalIgnoreCase) is true ? "asc" : "desc";
}
