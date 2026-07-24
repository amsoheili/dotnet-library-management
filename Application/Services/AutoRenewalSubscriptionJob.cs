using library_management.Data;
using Microsoft.EntityFrameworkCore;

public class AutoRenewalSubscription(
    ILogger<AutoRenewalSubscription> _logger,
    IServiceScopeFactory _scopeFactory
) : BackgroundService
{
    double JobPeriodMillisencods = TimeSpan.FromHours(1).TotalMilliseconds;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            _logger.LogWarning("Running Auto Renewal Subscriptions Job");
            await ProcessAsync(ct);
            await Task.Delay((int)JobPeriodMillisencods, ct);
        }
    }

    private async Task ProcessAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var _userSubscriptionService = scope.ServiceProvider.GetRequiredService<IUserSubscriptionService>();
            var _paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
            var renewalSubscriptions = await _db.UserSubscriptions
                .Where(s =>
                    s.AutoRenewal == true &&
                    s.Status == UserSubscriptionStatus.Active &&
                    s.EndAt < DateTime.UtcNow
                    )
                .ToListAsync(ct);

            foreach (var subscription in renewalSubscriptions)
            {
                await using var transaction = await _db.Database.BeginTransactionAsync(ct);
                try
                {
                    var paymentInvoice = await _userSubscriptionService.CreateSubscriptionInvoice(
                                        subscription.LibraryUserId,
                                        subscription.LibrarySubscriptionId,
                                        new(subscription.BillingPeriod, true),
                                        ct
                                    );

                    var succeeded = await _paymentService.VerifyByWallet(subscription.LibraryUserId, paymentInvoice.Id, ct);
                    if (!succeeded)
                    {
                        _logger.LogWarning("Reactivating subscription failed, userid: " + subscription.LibraryUserId + " subscription id: " + subscription.LibrarySubscriptionId);
                        subscription.AutoRenewal = false;
                    }
                    await _db.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync(ct);
                    _logger.LogError(e, "Reactivating subscription failed, userid: " + subscription.LibraryUserId + " subscription id: " + subscription.LibrarySubscriptionId);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error happend during the renewal job");
        }
    }
}