using SupportHub.Application.DTOs.Responses.Tickets;

namespace SupportHub.Application.Features.Tickets.Queries.Tickets.GetTicketDetail;

public record GetTicketDetailQueryResponse(
    Guid Id, 
    string Title, 
    string Description, 
    string Status, 
    string Priority, 
    DateTime CreatedDate, 
    DateTime? UpdatedDate, 
    List<ResponseTicketComment> Comments,
    List<ResponseTicketActivity> Activities);