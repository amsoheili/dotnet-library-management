using library_management.Data;
using Microsoft.EntityFrameworkCore;

public class AutoRenewalSubscription(
    ILogger<AutoRenewalSubscription> _logger,
    IServiceScopeFactory _scopeFactory
) : BackgroundService
{
    int JobPeriodMillisencods = TimeSpan.FromMinutes(1).Milliseconds;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            _logger.LogWarning("Running Auto Renewal Subscriptions Job");
            await ProcessAsync(ct);
            await Task.Delay(JobPeriodMillisencods, ct);
        }
    }

    private async Task ProcessAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userSubscriptions = await db.UserSubscriptions
                .Where(s =>
                    s.AutoRenewal == true &&
                    s.Status == UserSubscriptionStatus.Active &&
                    s.EndAt > DateTime.UtcNow
                    )
                .ToListAsync(ct);

            foreach (var subscription in userSubscriptions)
            {
                
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}