using library_management.Entities;

public class LibraryUser : Person
{
    public string Username { get; set; }
    public string HashedPassword { get; set; }
    public string? LibraryId { get; set; }
    public Library? Library { get; set; }
    public DateTime? MembershipStart { get; set; }
    public List<Book>? BorrowedBooks { get; set; }
    public List<Book>? FavoriteBooks { get; set; }
}