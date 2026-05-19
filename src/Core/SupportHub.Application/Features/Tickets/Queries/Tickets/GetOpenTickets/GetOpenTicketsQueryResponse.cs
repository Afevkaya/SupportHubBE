using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetOpenTickets;

public record GetOpenTicketsQueryResponse(List<ResponseGetTicket> Items, int Page, int PageSize, int TotalCount, int TotalPages);