using SupportHub.Application.DTOs.Responses.Auths;

namespace SupportHub.Application.Features.Tickets.Queries.GetTicketComments;

public record GetTicketCommentsQueryResponse(Guid Id, Guid TicketId, string Message, DateTime CreatedDate, ResponseGetAuth Author);