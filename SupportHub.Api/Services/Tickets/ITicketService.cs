using SupportHub.Api.Models.Requests;
using SupportHub.Api.Models.Responses;

namespace SupportHub.Api.Services.Tickets;

public interface ITicketService
{
    Task<List<ResponseGetTicket>> GetTicketsAsync();
    Task<ResponseCreateTicket> CreateTicketAsync(RequestCreateTicket request);
}