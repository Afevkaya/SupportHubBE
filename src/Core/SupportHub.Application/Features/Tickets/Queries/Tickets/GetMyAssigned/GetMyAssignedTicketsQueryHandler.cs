using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetMyAssigned;

public class GetMyAssignedTicketsQueryHandler(
    ITicketReadRepository readRepository,
    ICurrentService currentService,
    ILogger<GetMyAssignedTicketsQueryHandler> logger) : IRequestHandler<GetMyAssignedTicketsQuery, List<GetMyAssignedTicketsQueryResponse>>
{
    public async Task<List<GetMyAssignedTicketsQueryResponse>> Handle(GetMyAssignedTicketsQuery request, CancellationToken cancellationToken)
    {
        var assignedTicket = await readRepository.GetMyAssignedTicketsAsync(currentService.UserId!.Value, cancellationToken);
        logger.LogInformation("Assigned tickets listed. AgentId: {AgentId}, TicketCount: {TicketCount}", currentService.UserId.Value, assignedTicket.Count);
        return assignedTicket.Select(t => new GetMyAssignedTicketsQueryResponse(
            t.Id,
            t.Title,
            t.Status.ToString(),
            t.CreatedDate)).ToList();
    }
}