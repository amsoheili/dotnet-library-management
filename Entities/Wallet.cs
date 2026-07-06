using library_management.Entities;

public class Wallet : BaseEntity
{
    public double Balance { get; set; } = 0.0;

    public required string PersonId { get; set; }
    public Person Person { get; set; }
}