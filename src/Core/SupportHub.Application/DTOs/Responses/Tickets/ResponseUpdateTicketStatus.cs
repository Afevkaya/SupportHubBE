using SupportHub.Application.DTOs.Responses.Auths;

namespace SupportHub.Application.DTOs.Responses.Tickets;

public record ResponseUpdateTicketStatus(
    Guid Id, 
    string Title,
    string Description, 
    string Status, 
    DateTime CreatedDate, 
    DateTime? UpdatedDate,
    ResponseGetAuth? Author = null);