using SupportHub.Domain.Common;
using SupportHub.Domain.Enums;

namespace SupportHub.Domain.Entities;

public class Ticket : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public TicketStatusType Status { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public TicketPriorityType Priority { get; set; } = TicketPriorityType.Medium;
    public Guid? CreatedByUserId { get; set; }
    public ICollection<TicketComment> TicketComments { get; set; } = new List<TicketComment>();
    public ICollection<TicketActivity> TicketActivities { get; set; } = new List<TicketActivity>();
}