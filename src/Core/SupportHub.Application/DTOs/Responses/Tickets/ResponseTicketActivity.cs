namespace SupportHub.Application.DTOs.Responses.Tickets;

public record ResponseTicketActivity(
    string ActivityType,
    string Description,
    DateTime CreatedDate);