using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.Tickets;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;

public class GetAllTicketsQueryHandler(ITicketReadRepository ticketReadRepository,ILogger<GetAllTicketsQueryHandler> logger) : IRequestHandler<GetAllTicketsQuery, GetAllTicketsQueryResponse>
{
    public async Task<GetAllTicketsQueryResponse> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
    {
        var response = await ticketReadRepository.GetAllAsync(request.Page, request.PageSize,request.SortBy, request.SortDirection);
        logger.LogInformation(
            "Tickets retrieved. Page: {Page}, PageSize: {PageSize}, SortBy: {SortBy}, SortDirection: {SortDirection}, TotalCount: {TotalCount}, ReturnedItemCount: {ReturnedItemCount}",
            request.Page,
            request.PageSize,
            request.SortBy,
            request.SortDirection,
            response.TotalCount,
            response.Items.Count);
        return response;
    }
}