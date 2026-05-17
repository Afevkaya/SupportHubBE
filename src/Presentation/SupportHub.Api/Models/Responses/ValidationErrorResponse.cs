namespace SupportHub.Api.Models.Responses;

public record ValidationErrorResponse(int StatusCode, string Message, IDictionary<string, List<string>> Errors);