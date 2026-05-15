using SupportHub.Api.Models.Requests;
using SupportHub.Api.Models.Responses;

namespace SupportHub.Api.Services.Tickets;

public interface ITicketService
{
    Task<List<ResponseGetTicket>> GetTicketsAsync();
    Task<List<ResponseGetTicket>> GetOpenTicketsAsync();
    Task<ResponseCreateTicket> CreateTicketAsync(RequestCreateTicket request);
    Task<ResponseUpdateTicketStatus> UpdateTicketStatusAsync(Guid id, RequestUpdateTicketStatus request);
}