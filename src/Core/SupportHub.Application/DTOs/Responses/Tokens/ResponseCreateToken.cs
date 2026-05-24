namespace SupportHub.Application.DTOs.Responses.Tokens;

public class ResponseCreateToken
{
    public string AccessToken { get; set; }
    public DateTime ExpireAt { get; set; }
}