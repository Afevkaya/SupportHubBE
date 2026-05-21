using SupportHub.Application.Abstractions.Messaging;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetTicketDetail;

public record GetTicketDetailQuery(Guid Id) : IQuery<GetTicketDetailQueryResponse>;