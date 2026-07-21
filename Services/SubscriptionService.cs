using library_management.Data;
using Microsoft.EntityFrameworkCore;

public interface ISubscriptionService
{
    public Task<SubscriptionPurchaseResponseDto> Purchase(string id, PurchaseSubscriptionPlanDto purchaseSubscriptionPlanDto, CancellationToken ct);
}

public class SubscriptionService(
    IUserClaimsService _userClaimsService,
    AppDbContext _db,
    ILogger<SubscriptionService> _logger
) : ISubscriptionService
{
    public async Task<SubscriptionPurchaseResponseDto> Purchase(string id, PurchaseSubscriptionPlanDto purchaseSubscriptionPlanDto, CancellationToken ct)
    {
        _logger.LogWarning("hereee");
        var userId = _userClaimsService.GetUserId();

        if (userId is null)
            return new(null);

        var subscription = await _db.LibrarySubscriptions
                            .AsNoTracking()
                            .Include(s => s.Library)
                            .ThenInclude(l => l.Wallet)
                            .SingleOrDefaultAsync(s => s.Id == id, ct);

        if (subscription is null)
            return new(null);

        decimal amount;
        switch (purchaseSubscriptionPlanDto.billingPeriod)
        {
            case SubscriptionBillingPeriod.Monthly:
                amount = subscription.MonthlyCost;
                break;
            case SubscriptionBillingPeriod.Yearly:
                amount = subscription.YearlyCost;
                break;
            default:
                return new(null);
        }

        var invoice = new PaymentInvoice
        {
            PersonId = userId,
            CreatedAt = DateTime.UtcNow,
            Amount = amount,
            SubscriptionId = id,
            status = PaymentInvoiceStatusEnum.Draft,
            DestinationWalletId = subscription.Library.Wallet.Id
        };

        await _db.PaymentInvoices.AddAsync(invoice, ct);

        await _db.SaveChangesAsync(ct);

        return new(invoice.Id);
    }
}