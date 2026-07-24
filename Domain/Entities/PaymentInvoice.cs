using library_management.Entities;

public class PaymentInvoice : BaseEntity
{
    public PaymentMethodEnum? method { get; set; }

    public PaymentInvoiceStatusEnum status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime PaidAt { get; set; }

    public decimal Amount { get; set; } = 00.00m;

    public string PersonId { get; set; }
    public Person Person { get; set; }

    public string DestinationWalletId { get; set; }

    public InvoiceProductType ProductType { get; set; }
    public string ProductId { get; set; }

}