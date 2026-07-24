namespace library_management.Entities;

public class Book : BaseEntity
{
    public string Title { get; set; }
    public string? AuthorId { get; set; }
    public Author? Author { get; set; }
    public string? LibraryId { get; set; }
    public Library? Library { get; set; }
}