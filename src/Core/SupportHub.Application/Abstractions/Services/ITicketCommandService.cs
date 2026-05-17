using SupportHub.Application.DTOs.Requests;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Abstractions.Services;

public interface ITicketCommandService
{
    Task<ResponseUpdateTicketStatus> UpdateTicketStatusAsync(Guid id, RequestUpdateTicketStatus request);
}