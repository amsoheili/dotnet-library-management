using library_management.Data;
using Microsoft.EntityFrameworkCore;

public interface IUserSubscriptionService
{
    public Task<SubscriptionPurchaseResponseDto> Purchase(string subscriptionId, PurchaseSubscriptionPlanDto purchaseSubscriptionPlanDto, CancellationToken ct);
    public Task<bool> ActivateSubscription(string userSubscriptionId, CancellationToken ct);
}

public class UserSubscriptionService(
    IUserClaimsService _userClaimsService,
    AppDbContext _db,
    ILogger<UserSubscriptionService> _logger
) : IUserSubscriptionService
{
    public async Task<bool> ActivateSubscription(string userSubscriptionId, CancellationToken ct)
    {

        var pendingSubscription = await _db.UserSubscriptions
                                    .SingleOrDefaultAsync(s => s.Id == userSubscriptionId);

        if (pendingSubscription is null)
            return false;

        DateTime endAt = DateTime.UtcNow;
        switch (pendingSubscription.BillingPeriod)
        {
            case SubscriptionBillingPeriod.Monthly:
                endAt = DateTime.UtcNow + TimeSpan.FromDays(30);
                break;
            case SubscriptionBillingPeriod.Yearly:
                endAt = DateTime.UtcNow + TimeSpan.FromDays(365);
                break;
        }

        pendingSubscription.EndAt = endAt;

        pendingSubscription.Status = UserSubscriptionStatus.Active;

        await _db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<SubscriptionPurchaseResponseDto> Purchase(string subscriptionId, PurchaseSubscriptionPlanDto purchaseSubscriptionPlanDto, CancellationToken ct)
    {
        var userId = _userClaimsService.GetUserId();

        if (userId is null)
            return new(null);

        var userSubscriptions = await _db.UserSubscriptions
                       .Where(u => u.LibraryUserId == userId)
                       .ToListAsync(ct);

        var hasActiveSubscription = (userSubscriptions is not null) && userSubscriptions.Count > 0 ?
                                     userSubscriptions.Any(s => s.Status == UserSubscriptionStatus.Active) : false;

        if (hasActiveSubscription)
            return new(null);

        var pendingSubscriptions = await _db.UserSubscriptions
                                    .Where(s => s.LibraryUserId == userId && s.Status == UserSubscriptionStatus.PendingForPayment)
                                    .ToListAsync(ct);

        _db.UserSubscriptions.RemoveRange(pendingSubscriptions);

        var subscription = await _db.LibrarySubscriptions
                            .AsNoTracking()
                            .Include(s => s.Library)
                            .ThenInclude(l => l.Wallet)
                            .SingleOrDefaultAsync(s => s.Id == subscriptionId, ct);

        if (subscription is null)
            return new(null);

        DateTime endAt = DateTime.UtcNow;
        decimal amount;
        switch (purchaseSubscriptionPlanDto.billingPeriod)
        {
            case SubscriptionBillingPeriod.Monthly:
                amount = subscription.MonthlyCost;
                endAt = DateTime.UtcNow + TimeSpan.FromDays(30);
                break;
            case SubscriptionBillingPeriod.Yearly:
                amount = subscription.YearlyCost;
                endAt = DateTime.UtcNow + TimeSpan.FromDays(365);
                break;
            default:
                return new(null);
        }

        var userSubscription = new UserSubscription
        {
            LibraryUserId = userId,
            LibrarySubscriptionId = subscriptionId,
            BillingPeriod = purchaseSubscriptionPlanDto.billingPeriod,
            Status = UserSubscriptionStatus.PendingForPayment,
            StartAt = DateTime.UtcNow,
            EndAt = endAt,
            AutoRenewal = purchaseSubscriptionPlanDto.autoRenewal ?? false
        };

        await _db.UserSubscriptions.AddAsync(userSubscription);

        var invoice = new PaymentInvoice
        {
            PersonId = userId,
            CreatedAt = DateTime.UtcNow,
            Amount = amount,
            ProductType = InvoiceProductType.Subscription,
            ProductId = userSubscription.Id,
            status = PaymentInvoiceStatusEnum.Draft,
            DestinationWalletId = subscription.Library.Wallet.Id
        };

        await _db.PaymentInvoices.AddAsync(invoice, ct);

        await _db.SaveChangesAsync(ct);

        return new(invoice.Id);
    }
}