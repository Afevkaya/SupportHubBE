using MediatR;
using SupportHub.Application.DTOs.Responses;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetAllTickets;

public record GetAllTicketsQuery(int Page, int PageSize, string? Status, string Search, string SortBy, string SortDirection) : IRequest<GetAllTicketsQueryResponse>;