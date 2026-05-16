namespace SupportHub.Application.DTOs.Responses;

public record ResponseCreateTicket(Guid Id, string Title, string Description, string Status, DateTime CreatedDate);