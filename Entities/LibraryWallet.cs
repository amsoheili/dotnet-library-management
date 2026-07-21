using library_management.Entities;

public class LibraryWallet : Wallet
{
    public required string LibraryId { get; set; }
    public Library Library { get; set; }
}