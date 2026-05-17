using SupportHub.Application.Abstractions.Repositories.Tickets;
using SupportHub.Application.Abstractions.Services;
using SupportHub.Application.DTOs.Requests;
using SupportHub.Application.DTOs.Responses;
using SupportHub.Domain.Entities;
using SupportHub.Domain.Enums;

namespace SupportHub.Application.Services.Tickets;

public class TicketCommandService(ITicketWriteRepository ticketWriteRepository) : ITicketCommandService
{
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
            Status = TicketStatusType.Open,
            CreatedDate = DateTime.UtcNow
        };
        var result = await ticketWriteRepository.CreateAsync(ticket);
        var response = new ResponseCreateTicket(
            result.Id,
            result.Title,
            result.Description,
            result.Status.ToString(),
            result.CreatedDate
        );
        return response;
    }
    
    public async Task<ResponseUpdateTicketStatus> UpdateTicketStatusAsync(Guid id, RequestUpdateTicketStatus request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if(!Enum.IsDefined(request.Status))
            throw new ArgumentException("Invalid status value.");
        var ticket = await ticketWriteRepository.UpdateStatusAsync(id, request.Status);
        var response = new ResponseUpdateTicketStatus(
            ticket.Id,
            ticket.Title,
            ticket.Description,
            ticket.Status.ToString(),
            ticket.CreatedDate,
            ticket.UpdatedDate
        );
        return response;
    }
}