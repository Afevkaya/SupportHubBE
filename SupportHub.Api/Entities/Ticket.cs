namespace SupportHub.Api.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public DateTime CreatedDate { get; set; }
}