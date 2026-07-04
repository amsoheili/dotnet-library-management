using library_management.Data;
using library_management.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace library_management.Services;

public interface IBooksDataService
{
    public Task<List<Book>> GetBooks(PaginationDto pagination, CancellationToken ct);
    public Task<Book> GetBookById(string id, CancellationToken ct);
    public Task AddBook(Book book, CancellationToken ct);
    public Task DeleteBook(string id, CancellationToken ct);
    public Task<Book> UpdateBook(string id, Book book, CancellationToken ct);
    public Task<int> CountBooks(string regex, CancellationToken ct);
}

public class BooksDataService : IBooksDataService
{
    private readonly AppDbContext _context;
    private readonly ILogger<BooksDataService> _logger;

    public BooksDataService(
        AppDbContext context,
        ILogger<BooksDataService> logger
        )
    {
        _context = context;
        _logger = logger;
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

    public async Task<List<Book>> GetBooks(PaginationDto pagination, CancellationToken ct)
    {
        return await _context.Books
                        .AsNoTracking()
                        .Skip((pagination.page - 1) * pagination.pageSize)
                        .Take(pagination.pageSize)
                        .ToListAsync(ct);
    }

    public async Task DeleteBook(string id, CancellationToken ct)
    {
        await _context.Books.Where(b => b.Id == id).ExecuteDeleteAsync(ct);
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
    public async Task<int> CountBooks(string regex, CancellationToken ct)
    {
        int count = 0;
        try
        {
            _logger.LogInformation("About to bring the books in memory");
            var books = await _context.Books.ToListAsync(ct);
            _logger.LogInformation("All books brought into memory");

            foreach (Book book in books)
            {
                ct.ThrowIfCancellationRequested();
                if (book.Title.Contains(regex))
                    count++;
            }
        }
        catch (OperationCanceledException operationCanceledException)
        {
            _logger.LogWarning("Catching the operation cancelled exception");
            _logger.LogWarning(operationCanceledException.Message);
        }

        return count;
    }

}
