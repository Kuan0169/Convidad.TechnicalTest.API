namespace Convidad.TechnicalTest.API.Middlewares
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(RequestDelegate next,
            ILogger<GlobalExceptionHandler> logger)
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

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ArgumentException or ArgumentNullException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            var message = exception switch
            {
                KeyNotFoundException => "Resource not found",
                ArgumentException or ArgumentNullException => "Invalid request parameters",
                _ => "An unexpected error occurred"
            };

            var jsonResponse = $"{{\"message\":\"{message}\",\"statusCode\":{statusCode}}}";
            context.Response.StatusCode = statusCode;

            try
            {
                await context.Response.WriteAsync(jsonResponse);
            }
            catch (Exception writeEx)
            {
                var loggerFactory = context.RequestServices.GetService<ILoggerFactory>();
                if (loggerFactory != null)
                {
                    var logger = loggerFactory.CreateLogger<GlobalExceptionHandler>();
                    logger.LogWarning("Failed to write error response: {Message}", writeEx.Message);
                }
            }
        }
    }
}