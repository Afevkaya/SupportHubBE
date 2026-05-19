using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.Tickets;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetOpenTickets;

public class GetOpenTicketsQueryHandler(ITicketReadRepository ticketReadRepository, ILogger<GetOpenTicketsQueryHandler> logger) : IRequestHandler<GetOpenTicketsQuery, GetOpenTicketsQueryResponse>
{
    public async Task<GetOpenTicketsQueryResponse> Handle(GetOpenTicketsQuery request, CancellationToken cancellationToken)
    {
        var response = await ticketReadRepository.GetOpenTicketsAsync(request.Page, request.PageSize, cancellationToken: cancellationToken);
        logger.LogInformation(
            "Open tickets retrieved with pagination. Page: {Page}, PageSize: {PageSize}, SortBy: {SortBy}, SortDirection: {SortDirection}, TotalCount: {TotalCount}, ReturnedItemCount: {ReturnedItemCount}",
            request.Page,
            request.PageSize,
            request.SortBy,
            request.SortDirection,
            response.TotalCount,
            response.Items.Count);
        return response;
    }
}