namespace SupportHub.Application.DTOs.Responses;

public record ResponseTicketActivity(
    string ActivityType,
    string Description,
    DateTime CreatedDate);