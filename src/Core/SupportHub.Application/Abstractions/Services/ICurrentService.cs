namespace SupportHub.Application.Abstractions.Services;

public interface ICurrentService
{
    public bool IsAuthenticated  { get; }
    public string? Email { get; }
    public Guid? UserId { get; }
    
}