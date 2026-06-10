using library_management.Data;
using library_management.DTOs;
using library_management.Entities;
using library_management.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public interface ILibraryService
{
    public Task<bool> AddBookToLibraryAsync(string libraryId, string bookId);

    public Task<List<BookDTO>> GetLibraryBooksAsync(string id);

    public Task<string> CreateLibraryAsync(string fullname);

    public Task<List<LibraryDto>> GetAllLibrariesAsync();

    public Task<bool> LendBook(string libraryId, string bookId, string memberId);

    public Task<bool> AddMember(string libraryId, MemberDto member);

    public Task<List<MemberDto>> GetMembers(string libraryId);

    public Task<bool> RetakeBook(string libraryId, string bookId, string memberId);
}

public class LibraryService : ILibraryService
{
    private readonly AppDbContext _context;
    private readonly IHybridCacheService _hybridCache;
    private readonly ILogger<LibraryService> _logger;
    public LibraryService(
        AppDbContext context,
        IMemoryCache memoryCache,
        ILogger<LibraryService> logger,
        IRedisCacheService redisCache,
        IHybridCacheService hybridCache
         )
    {
        _context = context;
        _logger = logger;
        _hybridCache = hybridCache;
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
        var libraryList = await _hybridCache.GetEntry<List<LibraryDto>>(AppInMemoryCacheKeys.LibrariesList);

        if (libraryList is null)
        {
            libraryList = await _context.Libraries.Select(l => new LibraryDto(l.Id, l.FullName)).ToListAsync();

            await _hybridCache.SetEntry(AppInMemoryCacheKeys.LibrariesList, libraryList);

        }
        return libraryList;
    }

    public async Task<bool> LendBook(string libraryId, string bookId, string memberId)
    {
        var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == memberId && m.LibraryId == libraryId);

        if (member is null)
            return false;

        var book = await _context.Books
            .Where(b => b.LibraryId == libraryId)
            .FirstOrDefaultAsync(b => b.Id == bookId);

        if (book is null)
            return false;

        var hasLentBefore = await _context.BorrowedBooks.AnyAsync(bB => bB.BookId == bookId);

        if (hasLentBefore)
            return false;

        await _context.BorrowedBooks.AddAsync(new BorrowedBook
        {
            BookId = bookId,
            MemberId = memberId,
            LibraryId = libraryId,
            Date = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddMember(string libraryId, MemberDto member)
    {
        var libraryExists = await _context.Libraries.AnyAsync(l => l.Id == libraryId);

        if (!libraryExists)
            return false;

        var memberExists = await _context.Members.AnyAsync(m => m.NationalCode == member.NationalCode && m.LibraryId == libraryId);

        if (memberExists)
            return false;

        var newMember = new Member
        {
            FirstName = member.FirstName,
            LastName = member.LastName,
            NationalCode = member.NationalCode,
            PhoneNumber = member.PhoneNumber,
            LibraryId = libraryId
        };
        await _context.Members.AddAsync(newMember);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<MemberDto>> GetMembers(string libraryId)
    {
        return await _context.Members
        .Where(m => m.LibraryId == libraryId)
        .Select(m => new MemberDto(m.Id, m.FirstName, m.LastName, m.NationalCode, m.PhoneNumber))
        .ToListAsync();
    }

    public async Task<bool> RetakeBook(string libraryId, string bookId, string memberId)
    {
        var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == memberId && m.LibraryId == libraryId);

        if (member is null)
            return false;

        var borrowedBook = await _context.BorrowedBooks.FirstOrDefaultAsync(bB => bB.BookId == bookId && bB.LibraryId == libraryId);

        if (borrowedBook is null)
            return false;

        _context.Remove(borrowedBook);

        await _context.SaveChangesAsync();

        return true;
    }
}
