using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetTicketComments;

public record GetTicketCommentsQuery(Guid TicketId) : IQuery<List<GetTicketCommentsQueryResponse>>;