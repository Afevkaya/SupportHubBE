using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Services.Tickets;

public class TicketQueryService(ITicketReadRepository ticketReadRepository) : ITicketQueryService
{
    public async Task<List<ResponseGetTicket>> GetTicketsAsync()
    {
        var result = await ticketReadRepository.GetAllAsync();
        if(result.Count == 0)
            throw new ArgumentException("No tickets found.");
        var response = result.Select(x => new ResponseGetTicket(x.Id, x.Title,x.Status.ToString(), x.CreatedDate)).ToList();
        return response;
    }
}