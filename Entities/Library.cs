using library_management.Entities;

public class Library : BaseEntity
{
    public string FullName { get; set; }
    public Person? Librarian { get; set; }
    public List<Book>? Books { get; set; }
    public List<Person>? Members { get; set; }
    public List<BorrowedBook>? BorrowedBooks { get; set; }
}