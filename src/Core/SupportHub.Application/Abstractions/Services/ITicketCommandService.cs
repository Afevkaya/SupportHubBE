using SupportHub.Application.DTOs.Requests;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Abstractions.Services;

public interface ITicketCommandService
{
    Task<ResponseCreateTicket> CreateTicketAsync(RequestCreateTicket request);
    Task<ResponseUpdateTicketStatus> UpdateTicketStatusAsync(Guid id, RequestUpdateTicketStatus request);
}