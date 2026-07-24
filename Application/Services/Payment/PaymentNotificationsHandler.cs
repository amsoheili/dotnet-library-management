using MediatR;

public class PaymentNotificationsHanlder(
    IUserSubscriptionService _userSubscriptionService
) : IRequestHandler<PaymentInvoicePaidNotification, bool>
{
    public async Task<bool> Handle(PaymentInvoicePaidNotification request, CancellationToken cancellationToken)
    {
        switch (request.ProductType)
        {
            case InvoiceProductType.Subscription:
                return await _userSubscriptionService.ActivateSubscription(request.ProductId, cancellationToken);
            default:
                return false;
        }
    }
}