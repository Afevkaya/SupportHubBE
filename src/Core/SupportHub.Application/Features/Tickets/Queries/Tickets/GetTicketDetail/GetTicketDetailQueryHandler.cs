using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.DTOs.Responses;
using SupportHub.Application.DTOs.Responses.Tickets;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetTicketDetail;

public class GetTicketDetailQueryHandler(
    ITicketReadRepository ticketReadRepository,
    ICurrentService currentService,
    ILogger<GetTicketDetailQueryHandler> logger) : IRequestHandler<GetTicketDetailQuery, GetTicketDetailQueryResponse>
{
    public async Task<GetTicketDetailQueryResponse> Handle(GetTicketDetailQuery request, CancellationToken cancellationToken)
    {        
        var ticket = await ticketReadRepository.GetTicketDetail(request.Id);
        if (ticket is not null)
            return new GetTicketDetailQueryResponse(
                ticket.Id,
                ticket.Title,
                ticket.Description,
                ticket.Status.ToString(),
                ticket.Priority.ToString(),
                ticket.CreatedDate,
                ticket.UpdatedDate,
                ticket.TicketComments.Select(c => new ResponseTicketComment(currentService?.FullName ?? string.Empty,c.Message)).ToList(),
                ticket.TicketActivities.Select(a => new ResponseTicketActivity(a.ActivityType.ToString(), a.Description, a.CreatedDate)).ToList()
            );
        logger.LogWarning("Ticket with id {Id} not found", request.Id);
        throw new KeyNotFoundException("Ticket not found");
    }
}