namespace ApplicationCore.Responses;

public class ErrorResponse
{
    public string Message { get; init; }
    public int StatusCode { get; init; }
    public IDictionary<string, string[]> Errors { get; init; } = new Dictionary<string, string[]>();
}