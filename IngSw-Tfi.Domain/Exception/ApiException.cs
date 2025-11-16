namespace IngSw_Tfi.Domain.Exception;

public class ApiException
{
    public int StatusCode { get; set; }
    public string? _message { get; set; }
    public string? _detail { get; set; }
    public ApiException(int statusCode, string? message = null, string? detail = null)
    {
        StatusCode = statusCode;
        _message = message;
        _detail = detail;
    }
}
