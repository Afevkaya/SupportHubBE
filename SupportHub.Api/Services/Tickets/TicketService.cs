using SupportHub.Api.Entities;
using SupportHub.Api.Enums;
using SupportHub.Api.Models.Requests;
using SupportHub.Api.Models.Responses;

namespace SupportHub.Api.Services.Tickets;

public class TicketService : ITicketService
{
    private static readonly List<Ticket> Tickets = [];
    
    public Task<List<ResponseGetTicket>> GetTicketsAsync()
    {
        var response = Tickets.Select(t => new ResponseGetTicket(
            t.Id,
            t.Title,
            t.Status,
            t.CreatedDate
        )).ToList();
        return Task.FromResult(response);
    }
    public Task<ResponseCreateTicket> CreateTicketAsync(RequestCreateTicket request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrEmpty(request.Title)) throw new ArgumentException("Title is required.");
            
        if(string.IsNullOrEmpty(request.Description)) throw new ArgumentException("Description is required.");
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Status = nameof(TicketStatusType.Open),
            CreatedDate = DateTime.UtcNow
        };
        Tickets.Add(ticket);
        var response = new ResponseCreateTicket(
            ticket.Id,
            ticket.Title,
            ticket.Description,
            ticket.Status,
            ticket.CreatedDate
        );
        return Task.FromResult(response);
    }
}