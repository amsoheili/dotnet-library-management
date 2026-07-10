using library_management.Entities;

public class Wallet : BaseEntity
{
    public decimal Balance { get; set; } = 00.00m;

    public required string PersonId { get; set; }
    public Person Person { get; set; }
}