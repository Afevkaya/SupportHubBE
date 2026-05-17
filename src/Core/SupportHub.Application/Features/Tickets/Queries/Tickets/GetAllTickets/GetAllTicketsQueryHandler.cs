using MediatR;
using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;

public class GetAllTicketsQueryHandler(ITicketReadRepository ticketReadRepository) : IRequestHandler<GetAllTicketsQuery, List<ResponseGetTicket>>
{
    public async Task<List<ResponseGetTicket>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
    {
        var result = await ticketReadRepository.GetAllAsync();
        if(result.Count == 0)
            throw new ArgumentException("No tickets found.");
        var response = result.Select(x => new ResponseGetTicket(x.Id, x.Title,x.Status.ToString(), x.CreatedDate)).ToList();
        return response;
    }
}