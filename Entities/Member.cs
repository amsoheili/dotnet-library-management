using library_management.Entities;

public class Member : Person
{
    public string LibraryId { get; set; }
    public Library Library { get; set; }
    public DateTime MembershipStart { get; set; }
    public List<Book>? BorrowedBooks { get; set; }
    public List<Book>? FavoriteBooks { get; set; }
}