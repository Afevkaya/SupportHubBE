using SupportHub.Domain.Common;
using SupportHub.Domain.Enums;

namespace SupportHub.Domain.Entities;

public class TicketActivity : BaseEntity
{
    public TicketActivityType ActivityType { get; set; }
    public string Description { get; set; }
    public Guid? ActorUserId { get; set; }
    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; }
}