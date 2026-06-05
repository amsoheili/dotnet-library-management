using library_management.Entities;

public class BorrowedBook : BaseEntity
{
    public string BookId { get; set; }
    public Book Book { get; set; }
    public string MemberId { get; set; }
    public Member Member { get; set; }
    public string LibraryId { get; set; }
    public Library Library { get; set; }
    public DateTime Date { get; set; }
}