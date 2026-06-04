using library_management.Data;
using library_management.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace library_management.Services;

public interface IBooksDataService
{
    public Task<List<Book>> GetBooks();
    public Task<Book> GetBookById(string id);
    public Task AddBook(Book book);
    public Task DeleteBook(string id);
    public Task<Book> UpdateBook(string id, Book book);
}

public class BooksDataService : IBooksDataService
{
    private readonly AppDbContext _context;

    public BooksDataService(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddBook(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
    }

    public async Task<Book> GetBookById(string id)
    {
        return await _context.Books.SingleOrDefaultAsync(b => String.Equals(b.Id, id));
    }

    public async Task<List<Book>> GetBooks()
    {
        return await _context.Books.AsNoTracking().ToListAsync();
    }

    public async Task DeleteBook(string id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null) return;
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }

    public async Task<Book> UpdateBook(string id, Book book)
    {
        var retrievedBook = await _context.Books.FindAsync(id);
        if (retrievedBook is null) return new Book();
        retrievedBook.Title = book.Title;
        retrievedBook.Author = book.Author;
        await _context.SaveChangesAsync();
        return book;
    }

}
