namespace SupportHub.Application.DTOs.Responses;

public record ResponseGetTicket(Guid Id, string Title, string Status, string Priority, DateTime CreatedDate);