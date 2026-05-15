using SupportHub.Api.Entities;
using SupportHub.Api.Enums;
using SupportHub.Api.Models.Requests;
using SupportHub.Api.Models.Responses;
using SupportHub.Api.Repositories.Tickets;

namespace SupportHub.Api.Services.Tickets;

public class TicketService(ITicketRepository ticketRepository) : ITicketService
{
    
    public async Task<List<ResponseGetTicket>> GetTicketsAsync()
    {
        var result = await ticketRepository.GetAllAsync();
        var response = result.Select(x => new ResponseGetTicket(x.Id, x.Title,x.Status, x.CreatedDate)).ToList();
        return response;
    }
    public async Task<ResponseCreateTicket> CreateTicketAsync(RequestCreateTicket request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrEmpty(request.Title)) 
            throw new ArgumentException("Title is required.");
            
        if(string.IsNullOrEmpty(request.Description)) 
            throw new ArgumentException("Description is required.");
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Status = nameof(TicketStatusType.Open),
            CreatedDate = DateTime.UtcNow
        };
        var result = await ticketRepository.CreateAsync(ticket);
        var response = new ResponseCreateTicket(
            result.Id,
            result.Title,
            result.Description,
            result.Status,
            result.CreatedDate
        );
        return response;
    }
    public async Task<ResponseUpdateTicketStatus> UpdateTicketStatusAsync(Guid id, RequestUpdateTicketStatus request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if(!Enum.IsDefined(request.Status))
            throw new ArgumentException("Invalid status value.");
        var ticket = await ticketRepository.UpdateStatusAsync(id, request.Status);
        var response = new ResponseUpdateTicketStatus(
            ticket.Id,
            ticket.Title,
            ticket.Description,
            ticket.Status,
            ticket.CreatedDate,
            ticket.UpdatedDate
        );
        return response;
    }
}