public class PersonWallet : Wallet
{
    public required string PersonId { get; set; }
    public Person Person { get; set; }
}