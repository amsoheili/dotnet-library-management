using library_management.Entities;

public class WalletTransaction : BaseEntity
{
    public decimal Amount { get; set; } = 00.00m;

    public DateTime Date { get; set; }

    public TransactionTypeEnum TransactionType { get; set; }

    public string WalletId { get; set; }
    public Wallet Wallet { get; set; }
}