using SupportHub.Api.Enums;

namespace SupportHub.Api.Models.Requests;

public record RequestUpdateTicketStatus(TicketStatusType Status);
