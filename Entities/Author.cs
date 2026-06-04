using library_management.Entities;

public class Author : Person
{
    public List<Book>? WrittenBooks { get; set; }
}