using SupportHub.Api.Enums;

namespace SupportHub.Api.Models.Responses;

public record ResponseCreateTicket(Guid Id, string Title, string Description, string Status, DateTime CreatedDate);