using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace library_management.Filters;

public class ExecutionTimeFilter : IAsyncActionFilter
{
    private ILogger<ExecutionTimeFilter> _logger;

    public ExecutionTimeFilter(
        ILogger<ExecutionTimeFilter> logger
    )
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();

        var executedContext = await next();

        stopwatch.Stop();

        if (executedContext.ActionDescriptor is ControllerActionDescriptor descriptor)
        {
            _logger.LogInformation($"Executed {descriptor.MethodInfo.Name} method in {descriptor.ControllerName} controller in {stopwatch.ElapsedMilliseconds} milliseconds");
        }
    }
}