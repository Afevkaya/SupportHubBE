using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetOpenTickets;

public class GetOpenTicketsQueryHandler(ITicketReadRepository ticketReadRepository, ILogger<GetOpenTicketsQueryHandler> logger) : IRequestHandler<GetOpenTicketsQuery, GetOpenTicketsQueryResponse>
{
    public async Task<GetOpenTicketsQueryResponse> Handle(GetOpenTicketsQuery request, CancellationToken cancellationToken)
    {
        var result = await ticketReadRepository.GetOpenTicketsAsync(request.Page, request.PageSize, cancellationToken);
        logger.LogInformation(
            "Open tickets retrieved with pagination. Page: {Page}, PageSize: {PageSize}, TotalCount: {TotalCount}",
            request.Page,
            request.PageSize,
            result.TotalCount);
        return result.Items.Count == 0 ? new GetOpenTicketsQueryResponse([], request.Page, request.PageSize, 0, 0) : result;
    }
}