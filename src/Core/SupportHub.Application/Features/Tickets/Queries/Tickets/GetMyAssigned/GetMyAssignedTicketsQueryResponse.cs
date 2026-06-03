namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetMyAssigned;

public record GetMyAssignedTicketsQueryResponse(Guid Id, string Title, string Status, DateTime CreatedDate);