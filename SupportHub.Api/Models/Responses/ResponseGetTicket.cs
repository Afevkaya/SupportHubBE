namespace SupportHub.Api.Models.Responses;

public record ResponseGetTicket(
    Guid Id,
    string Title,
    string Status,
    DateTime CreatedDate
);