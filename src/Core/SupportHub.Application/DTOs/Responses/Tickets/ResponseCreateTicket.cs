using SupportHub.Application.DTOs.Responses.Auths;

namespace SupportHub.Application.DTOs.Responses.Tickets;

public record ResponseCreateTicket(
    Guid Id, 
    string Title, 
    string Description, 
    string Status, 
    string Priority, 
    DateTime CreatedDate,
    ResponseGetAuth? Author = null);