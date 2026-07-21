using library_management.Entities;

public class Library : BaseEntity
{
    public string FullName { get; set; }
    public Person? Librarian { get; set; }
    public List<Book>? Books { get; set; }
    public List<LibraryUser>? Members { get; set; }
    public List<BorrowedBook>? LentBooks { get; set; }
    public List<LibrarySubscription>? LibrarySubscriptionPlan { get; set; }
    public LibraryWallet Wallet { get; set; }

    public Library()
    {
        Wallet = new LibraryWallet
        {
            LibraryId = Id,
            Balance = 0.0000m
        };
    }
}