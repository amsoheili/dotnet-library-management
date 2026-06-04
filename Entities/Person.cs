using library_management.Entities;

public class Person : BaseEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string NationalCode { get; set; }
    public string PhoneNumber { get; set; }
    public string? Address { get; set; }
    public List<Book>? BorrowedBooks { get; set; }
    public List<Book>? FavoriteBooks { get; set; }
}