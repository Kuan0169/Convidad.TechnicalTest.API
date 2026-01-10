using Convidad.TechnicalTest.API.DTOs.Error;
using Microsoft.AspNetCore.Diagnostics;

namespace Convidad.TechnicalTest.API.Middlewares

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    public GlobalExceptionHandler(RequestDelegate next)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            KeyNotFoundException => new ErrorResponse(
                message: "Resource not found",
                detail: exception.Message,
                statusCode: StatusCodes.Status404NotFound),

            ArgumentException or ArgumentNullException => new ErrorResponse(
                message: "Invalid request parameters",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest),

            _ => new ErrorResponse(
                message: "An unexpected error occurred",
                detail: appEnvironment.IsDevelopment() ? exception.ToString() : null,
                statusCode: StatusCodes.Status500InternalServerError)
        };

        var appEnvironment = context.RequestServices.GetRequiredService<IWebHostEnvironment>();

        context.Response.StatusCode = response.StatusCode;
        return context.Response.WriteAsJsonAsync(response);
    }
}
