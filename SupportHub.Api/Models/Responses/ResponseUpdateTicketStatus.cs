namespace SupportHub.Api.Models.Responses;

public record ResponseUpdateTicketStatus(Guid Id, string Title, string Description, string Status, DateTime CreatedDate, DateTime? UpdatedDate);