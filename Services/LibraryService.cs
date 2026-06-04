using library_management.Data;
using library_management.DTOs;
using Microsoft.EntityFrameworkCore;

public interface ILibraryService
{
    public Task<bool> AddBookToLibraryAsync(string libraryId, string bookId);

    public Task<List<BookDTO>> GetLibraryBooksAsync(string id);

    public Task<string> CreateLibraryAsync(string fullname);

    public Task<List<LibraryDto>> GetAllLibrariesAsync();
}

public class LibraryService : ILibraryService
{
    private readonly AppDbContext _context;

    public LibraryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddBookToLibraryAsync(string libraryId, string bookId)
    {
        var libraryExists = await _context.Libraries.AnyAsync(l => l.Id == libraryId);

        if (!libraryExists)
            return false;

        var book = await _context.Books.SingleOrDefaultAsync(b => b.Id == bookId);

        if (book is null)
            return false;

        book.LibraryId = libraryId;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<BookDTO>> GetLibraryBooksAsync(string id)
    {
        return await _context.Books
                .Where(b => b.LibraryId == id)
                .Select(b => new BookDTO(b.Id, b.Title, b.AuthorId))
                .ToListAsync();
    }

    public async Task<string> CreateLibraryAsync(string fullname)
    {
        var library = new Library { FullName = fullname };
        await _context.Libraries.AddAsync(library);
        await _context.SaveChangesAsync();
        return library.Id;
    }

    public async Task<List<LibraryDto>> GetAllLibrariesAsync()
    {
        return await _context.Libraries.Select(l => new LibraryDto(l.Id, l.FullName)).ToListAsync();
    }
}
