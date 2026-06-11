using library_management.Data;
using library_management.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace library_management.Services;

public interface IBooksDataService
{
    public Task<List<Book>> GetBooks(CancellationToken ct);
    public Task<Book> GetBookById(string id, CancellationToken ct);
    public Task AddBook(Book book, CancellationToken ct);
    public Task DeleteBook(string id, CancellationToken ct);
    public Task<Book> UpdateBook(string id, Book book, CancellationToken ct);
}

public class BooksDataService : IBooksDataService
{
    private readonly AppDbContext _context;

    public BooksDataService(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddBook(Book book, CancellationToken ct)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Book> GetBookById(string id, CancellationToken ct)
    {
        return await _context.Books.SingleOrDefaultAsync(b => String.Equals(b.Id, id), ct);
    }

    public async Task<List<Book>> GetBooks(CancellationToken ct)
    {
        return await _context.Books.AsNoTracking().ToListAsync(ct);
    }

    public async Task DeleteBook(string id, CancellationToken ct)
    {
        var book = await _context.Books.FindAsync(id, ct);
        if (book is null) return;
        _context.Books.Remove(book);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Book> UpdateBook(string id, Book book, CancellationToken ct)
    {
        var retrievedBook = await _context.Books.FindAsync(id, ct);
        if (retrievedBook is null) return new Book();
        retrievedBook.Title = book.Title;
        retrievedBook.Author = book.Author;
        await _context.SaveChangesAsync(ct);
        return book;
    }

}
