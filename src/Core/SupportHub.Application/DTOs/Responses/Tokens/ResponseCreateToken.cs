namespace SupportHub.Application.DTOs.Responses.Tokens;

public class ResponseCreateToken
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpirationAt { get; set; }
}
