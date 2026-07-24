using library_management.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

public interface IPaymentService
{
    public Task<bool> VerifyByWallet(string? userId, string invoiceId, CancellationToken ct);
}

public class PaymentService(
    AppDbContext _db,
    IUserClaimsService _userClaimsService,
    ILogger<PaymentService> _logger,
    IMediator _mediator
) : IPaymentService
{

    // get request
    // return the data of invoice

    // verify the payment of invoice by wallet
    public async Task<bool> VerifyByWallet(string? userId, string invoiceId, CancellationToken ct)
    {
        var claimUserId = _userClaimsService.GetUserId();

        if (userId is null)
        {
            if (claimUserId is null)
                return false;

            userId = claimUserId;
        }

        await using var transaction = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            var invoice = await _db.PaymentInvoices.SingleOrDefaultAsync(i =>
                            i.Id == invoiceId &&
                            i.PersonId == userId &&
                            i.status == PaymentInvoiceStatusEnum.Draft, ct);


            if (invoice is null)
                return false;

            var userWallet = await _db.PersonWallets.SingleOrDefaultAsync(w => w.PersonId == userId);

            if (userWallet is null || userWallet.Balance < invoice.Amount)
                return false;

            var destinationWallet = await _db.LibraryWallets.SingleOrDefaultAsync(w => w.Id == invoice.DestinationWalletId);

            if (destinationWallet is null)
                return false;

            userWallet.Balance -= invoice.Amount;

            destinationWallet.Balance += invoice.Amount;

            invoice.status = PaymentInvoiceStatusEnum.Paid;


            var succeeded = await _mediator.Send(new PaymentInvoicePaidNotification
            {
                InvoiceId = invoiceId,
                PaidAt = DateTime.Now,
                ProductType = invoice.ProductType,
                ProductId = invoice.ProductId
            });

            if (!succeeded)
                return false;

            await _db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            await transaction.RollbackAsync(ct);
            return false;
        }
    }
}