using SupportHub.Domain.Enums;

namespace SupportHub.Application.DTOs.Requests;

public record RequestUpdateTicketStatus(TicketStatusType Status);