using SupportHub.Application.DTOs.Responses.Tickets;

namespace SupportHub.Application.Features.Tickets.Queries.GetAllTickets;

public record GetAllTicketsQueryResponse(List<ResponseGetTicket> Items, int Page, int PageSize, int TotalCount, int TotalPages);