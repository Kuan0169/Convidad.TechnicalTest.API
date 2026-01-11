using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

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
            // 設定基本回應屬性
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

            // 建立簡單的 JSON 物件
            var jsonResponse = $"{{\"message\":\"{message}\",\"statusCode\":{statusCode}}}";

            // 設定狀態碼
            context.Response.StatusCode = statusCode;

            try
            {
                // 使用最基礎的寫入方式
                await context.Response.WriteAsync(jsonResponse);
            }
            catch
            {
                // 如果寫入失敗，至少確保狀態碼正確
                // 不要拋出新例外
            }
        }
    }
}