namespace Convidad.TechnicalTest.API.DTOs.Error
public record ErrorResponse(
    string Message,
    string? Details = null,
    int StatusCode = 500  
);
