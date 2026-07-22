using MediatR;

public class PaymentInvoicePaidNotification : IRequest<bool>
{
    public string InvoiceId { get; set; }

    public DateTime PaidAt { get; set; }

    public InvoiceProductType ProductType { get; set; }

    public string ProductId { get; set; }
}