using library_management.Entities;

public class BorrowedBook : BaseEntity
{
    public Book Book { get; set; }
    public Person Person { get; set; }
    public DateTime Date { get; set; }

}