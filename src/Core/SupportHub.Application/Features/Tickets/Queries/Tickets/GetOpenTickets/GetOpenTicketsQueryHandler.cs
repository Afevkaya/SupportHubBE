using MediatR;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetOpenTickets;

public class GetOpenTicketsQueryHandler(ITicketReadRepository ticketReadRepository) : IRequestHandler<GetOpenTicketsQuery, List<ResponseGetTicket>>
{
    public async Task<List<ResponseGetTicket>> Handle(GetOpenTicketsQuery request, CancellationToken cancellationToken)
    {
        var result = await ticketReadRepository.GetOpenTicketsAsync();
        return result.Count == 0 ? [] : result;
    }
}