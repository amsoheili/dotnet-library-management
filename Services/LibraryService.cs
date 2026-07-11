using library_management.Data;
using library_management.DTOs;
using library_management.Entities;
using library_management.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public interface ILibraryService
{
    public Task<bool> AddBookToLibraryAsync(string libraryId, string bookId, CancellationToken ct);

    public Task<List<BookDTO>> GetLibraryBooksAsync(string id, string? search, PaginationDto? pagination, CancellationToken ct);

    public Task<string> CreateLibraryAsync(string fullname, CancellationToken ct);

    public Task<List<LibraryDto>> GetAllLibrariesAsync(string? search, PaginationDto? pagination, CancellationToken ct);

    public Task<bool> LendBook(string libraryId, string bookId, string memberId, CancellationToken ct);

    public Task<bool> AddMember(string libraryId, MemberDto member, CancellationToken ct);

    public Task<List<LibraryUserDto>> GetMembers(string libraryId, string? search, PaginationDto pagination, CancellationToken ct);

    public Task<bool> RetakeBook(string libraryId, string bookId, string memberId, CancellationToken ct);

    public Task<bool> AddSubscriptionPlan(string libraryId, AddLibrarySubscriptionPlanDto subscriptionPlan, CancellationToken ct);
}

public class LibraryService : ILibraryService
{
    private readonly AppDbContext _context;
    private readonly IHybridCacheService _hybridCache;
    private readonly ILogger<LibraryService> _logger;
    public LibraryService(
        AppDbContext context,
        ILogger<LibraryService> logger,
        IHybridCacheService hybridCache
         )
    {
        _context = context;
        _logger = logger;
        _hybridCache = hybridCache;
    }

    public async Task<bool> AddBookToLibraryAsync(string libraryId, string bookId, CancellationToken ct)
    {
        var libraryExists = await _context.Libraries.AsNoTracking().AnyAsync(l => l.Id == libraryId, ct);

        if (!libraryExists)
            return false;

        var book = await _context.Books.SingleOrDefaultAsync(b => b.Id == bookId, ct);

        if (book is null)
            return false;

        book.LibraryId = libraryId;

        await _context.SaveChangesAsync(ct);

        return true;
    }

    public async Task<List<BookDTO>> GetLibraryBooksAsync(string id, string? search, PaginationDto? pagination, CancellationToken ct)
    {
        var query = _context.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(b => b.Title.Contains(search));

        return await query
            .Where(b => b.LibraryId == id)
            .Skip((pagination.page - 1) * pagination.pageSize)
            .Take(pagination.pageSize)
            .Select(b => new BookDTO(b.Id, b.Title, b.AuthorId))
            .ToListAsync(ct);
    }

    public async Task<string> CreateLibraryAsync(string fullname, CancellationToken ct)
    {
        var library = new Library { FullName = fullname };
        await _context.Libraries.AddAsync(library, ct);
        await _context.SaveChangesAsync(ct);
        return library.Id;
    }


    public async Task<List<LibraryDto>> GetAllLibrariesAsync(string? search, PaginationDto? pagination, CancellationToken ct)
    {
        var hasSearch = !string.IsNullOrWhiteSpace(search);
        var hasPagination = pagination is not null && pagination.page > 0 && pagination.pageSize > 0;

        var readFromCache = !hasSearch && !hasPagination;

        List<LibraryDto> libraryList;

        if (readFromCache)
        {
            libraryList = await _hybridCache.GetEntry<List<LibraryDto>>(AppInMemoryCacheKeys.LibrariesList);

            if (libraryList is null)
            {
                libraryList = await _context.Libraries.Select(l => new LibraryDto(l.Id, l.FullName)).ToListAsync(ct);

                await _hybridCache.SetEntry(AppInMemoryCacheKeys.LibrariesList, libraryList);

            }
        }
        else
        {
            _logger.LogWarning("reading directly");
            var query = _context.Libraries.AsNoTracking();

            if (hasSearch)
            {
                _logger.LogWarning("has search");
                query = query.Where(l => l.FullName.Contains(search));
            }

            if (hasPagination)
            {
                _logger.LogWarning($"has pagination {pagination.page} {pagination.pageSize}");
                query = query.Skip((pagination.page - 1) * pagination.pageSize)
                                 .Take(pagination.pageSize);
            }


            libraryList = await query.Select(l => new LibraryDto(l.Id, l.FullName)).ToListAsync(ct);
        }

        return libraryList;
    }

    public async Task<bool> LendBook(string libraryId, string bookId, string memberId, CancellationToken ct)
    {
        var member = await _context.LibraryUsers.FirstOrDefaultAsync(m => m.Id == memberId && m.LibraryId == libraryId, ct);

        if (member is null)
            return false;

        var book = await _context.Books
            .Where(b => b.LibraryId == libraryId)
            .FirstOrDefaultAsync(b => b.Id == bookId, ct);

        if (book is null)
            return false;

        var hasLentBefore = await _context.BorrowedBooks.AnyAsync(bB => bB.BookId == bookId, ct);

        if (hasLentBefore)
            return false;

        await _context.BorrowedBooks.AddAsync(new BorrowedBook
        {
            BookId = bookId,
            LibraryUserId = memberId,
            LibraryId = libraryId,
            Date = DateTime.UtcNow
        }, ct);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> AddMember(string libraryId, MemberDto memberDto, CancellationToken ct)
    {
        var libraryExists = await _context.Libraries.AnyAsync(l => l.Id == libraryId, ct);

        if (!libraryExists)
            return false;

        var libraryUser = await _context.LibraryUsers
                        .SingleOrDefaultAsync(u => u.Id == memberDto.userId);

        if (libraryUser is null)
            return false;

        libraryUser.LibraryId = memberDto.libraryId;

        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<LibraryUserDto>> GetMembers(string libraryId, string? search, PaginationDto pagination, CancellationToken ct)
    {
        var query = _context.LibraryUsers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(m => m.Username.Contains(search) || m.FirstName.Contains(search) || m.LastName.Contains(search));

        return await query
        .Where(m => m.LibraryId == libraryId)
        .Skip((pagination.page - 1) * pagination.pageSize)
        .Take(pagination.page)
        .Select(m => new LibraryUserDto(m.Id, m.Username, m.FirstName, m.LastName, m.NationalCode, m.PhoneNumber))
        .ToListAsync(ct);
    }

    public async Task<bool> RetakeBook(string libraryId, string bookId, string memberId, CancellationToken ct)
    {
        var member = await _context.LibraryUsers.FirstOrDefaultAsync(m => m.Id == memberId && m.LibraryId == libraryId, ct);

        if (member is null)
            return false;

        await _context.BorrowedBooks.Where(bB => bB.Id == bookId && bB.LibraryId == libraryId).ExecuteDeleteAsync(ct);

        await _context.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> AddSubscriptionPlan(string libraryId, AddLibrarySubscriptionPlanDto subscriptionPlan, CancellationToken ct)
    {
        var subscriptionExists = await _context.LibrarySubscriptions
                                    .AnyAsync(ls => ls.MonthlyCost == subscriptionPlan.monthlyCost && ls.YearlyCost == subscriptionPlan.yearlyCost, ct);
        if (subscriptionExists)
            return false;

        await _context.LibrarySubscriptions.AddAsync(new LibrarySubscription
        {
            MonthlyCost = subscriptionPlan.monthlyCost,
            YearlyCost = subscriptionPlan.yearlyCost,
            LibraryId = libraryId
        }, ct);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
