public class MaintenanceMiddleware(
    IConfiguration _config,
    RequestDelegate _next,
    ILogger<MaintenanceMiddleware> _logger
)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        var isMaintenance = bool.Parse(_config.GetSection(AppConfigurations.IsMaintenanceMode).Value);
        _logger.LogWarning(isMaintenance.ToString());
        if (isMaintenance)
        {
            ctx.Response.StatusCode = 500;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsJsonAsync(new ApiGeneralResponse<string> { Result = "سیستم در دست تعمیر است." });
            return;
        }

        await _next(ctx);
    }
}