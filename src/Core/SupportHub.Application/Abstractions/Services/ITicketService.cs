using SupportHub.Application.DTOs.Requests;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Abstractions.Services;

public interface ITicketService
{
    Task<List<ResponseGetTicket>> GetTicketsAsync();
    Task<List<ResponseGetTicket>> GetOpenTicketsAsync();
    Task<ResponseCreateTicket> CreateTicketAsync(RequestCreateTicket request);
    Task<ResponseUpdateTicketStatus> UpdateTicketStatusAsync(Guid id, RequestUpdateTicketStatus request);
}