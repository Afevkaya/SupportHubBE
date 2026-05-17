using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Abstractions.Services;

public interface ITicketQueryService
{
    Task<List<ResponseGetTicket>> GetTicketsAsync();
}