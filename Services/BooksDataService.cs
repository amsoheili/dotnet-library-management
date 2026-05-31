using library_management.Entities;

namespace library_management.Services;

public interface IBooksDataService
{
    public List<Book> GetBooks();
    public Book GetBookById(string id);
    public void AddBook(Book book);
    public void DeleteBook(string id);
    public Book UpdateBook(string id, Book book);
}

public class BooksDataService : IBooksDataService
{
    public List<Book> BookList = new List<Book>([
        new Book{Id = "1",Title = "b1", Author = "a1"},
        new Book{Id = "2",Title = "b2", Author = "a2"},
        new Book{Id = "3",Title = "b3", Author = "a3"},
        new Book{Id = "4",Title = "b4", Author = "a4"},
        new Book{Id = "5",Title = "b5", Author = "a5"},
    ]);

    public void AddBook(Book book)
    {
        BookList.Add(book);
    }

    public Book GetBookById(string id)
    {
        return BookList?.Find(b => b.Id == id);
    }

    public List<Book> GetBooks()
    {
        return BookList;
    }

    public void DeleteBook(string id)
    {
        BookList.RemoveAll(b => b.Id == id);
    }

    public Book UpdateBook(string id, Book book)
    {
        var existingBookIndex = BookList.FindIndex(b => b.Id == id);
        BookList[existingBookIndex] = book;
        return book;
    }

}
