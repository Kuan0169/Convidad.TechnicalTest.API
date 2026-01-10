namespace Convidad.TechnicalTest.API.Middlewares

public class RequestTiming
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTiming> _logger;
    private readonly double _slowRequestThresholdMs;
    public RequestTiming(
        RequestDelegate next, 
        ILogger<RequestTiming> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;

        _slowRequestThresholdMs = configuration
            .GetValue<double>("SlowRequestThresholdMs", 500);
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var startwatch = DateTime.UtcNow;
        var originalBodyStream = context.Respone.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            var duration = DateTime.UtcNow - startTime;
            var durationMs = duration.TotalMilliseconds;

            if (durationMs > _slowRequestThresholdMs)
            {
                _logger.LogWarning(
                    "Slow request detected: {Method} {Path} took {ElapsedMilliseconds} ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    durationMs);
            }
        }
        finally
        {
            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
