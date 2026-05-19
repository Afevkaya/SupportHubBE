using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;

public record GetAllTicketsQueryResponse(List<ResponseGetTicket> Items, int Page, int PageSize, int TotalCount, int TotalPages);