namespace Convidad.TechnicalTest.API.DTOs.Error
{ 
    public record ErrorResponse
    {
        public string Message { get; init; }
        public string? Detail { get; init; }
        public int StatusCode { get; init; }

        public ErrorResponse(string message, string? detail = null, int statusCode = 500)
        {
            Message = message;
            Detail = detail;
            StatusCode = statusCode;
        }
    }
}
