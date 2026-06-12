public class ApiLoggerMiddleware
{
    public readonly RequestDelegate _next;
    private readonly ILogger<ApiLoggerMiddleware> _logger;

    public ApiLoggerMiddleware(RequestDelegate next, ILogger<ApiLoggerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation($"Incoming request: {context.Request.Path} - {context.Request.Method}");
        await _next(context);
        _logger.LogInformation($"Outgoing request: {context.Response.StatusCode}");
    }
}