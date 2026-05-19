using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;

public class GetAllTicketsQueryHandler(ITicketReadRepository ticketReadRepository,ILogger<GetAllTicketsQueryHandler> logger) : IRequestHandler<GetAllTicketsQuery, GetAllTicketsQueryResponse>
{
    public async Task<GetAllTicketsQueryResponse> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
    {
        var result = await ticketReadRepository.GetAllAsync(request.Page, request.PageSize);
        logger.LogInformation(
            "Tickets retrieved with pagination. Page: {Page}, PageSize: {PageSize}, TotalCount: {TotalCount}",
            request.Page,
            request.PageSize,
            result.TotalCount);
        return result.Items.Count == 0 ? throw new ArgumentException("No tickets found.") : result;
    }
}