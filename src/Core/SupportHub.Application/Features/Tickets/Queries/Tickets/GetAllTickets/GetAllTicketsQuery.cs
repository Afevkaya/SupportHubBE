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
    : IQuery<GetAllTicketsQueryResponse> , ICacheableQuery
{
    public string CacheKey =>
        $"tickets_page_{Page}_size_{PageSize}_sort_{SortBy}_{SortDirection}";

    public TimeSpan Expiration =>
        TimeSpan.FromMinutes(2);
}