using MediatR;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetTicketDetail;

public record GetTicketDetailQuery(Guid Id) : IRequest<GetTicketDetailQueryResponse>;