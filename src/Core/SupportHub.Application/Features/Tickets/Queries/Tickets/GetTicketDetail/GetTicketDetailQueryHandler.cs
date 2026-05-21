using MediatR;
using Microsoft.Extensions.Logging;
using SupportHub.Application.Abstractions.Repositories.Tickets;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetTicketDetail;

public class GetTicketDetailQueryHandler(ITicketReadRepository ticketReadRepository,ILogger<GetTicketDetailQueryHandler> logger) : IRequestHandler<GetTicketDetailQuery, GetTicketDetailQueryResponse>
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
                ticket.UpdatedDate
            );
        logger.LogWarning("Ticket with id {Id} not found", request.Id);
        throw new KeyNotFoundException("Ticket not found");
    }
}