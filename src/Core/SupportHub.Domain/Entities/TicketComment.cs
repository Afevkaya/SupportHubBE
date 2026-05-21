using SupportHub.Domain.Common;

namespace SupportHub.Domain.Entities;

public class TicketComment : BaseEntity
{
    public string Message { get; set; }
    public string AuthorName { get; set; }
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; }
}