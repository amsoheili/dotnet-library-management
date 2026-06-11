using library_management.Data;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

public interface IAuthorService
{
    public Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto author, CancellationToken ct);

    public Task<List<AuthorDto>> GetAuthorsAsync(CancellationToken ct);
}

public class AuthorService : IAuthorService
{
    private AppDbContext _context;
    private ILogger<AuthorService> _logger;

    public AuthorService(AppDbContext context, ILogger<AuthorService> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto author, CancellationToken ct)
    {

        var newAuthor = new Author
        {
            FirstName = author.FirstName,
            LastName = author.LastName,
            NationalCode = author.NationalCode,
            PhoneNumber = author.PhoneNumber
        };
        await _context.Authors.AddAsync(newAuthor, ct);
        await _context.SaveChangesAsync(ct);
        return new AuthorDto(newAuthor.Id, newAuthor.FirstName + " " + newAuthor.LastName);
    }

    [OutputCache(Duration = 10)]
    public async Task<List<AuthorDto>> GetAuthorsAsync(CancellationToken ct)
    {
        return await _context.Authors.Select(a => new AuthorDto(
            a.Id,
            $"{a.FirstName} {a.LastName}"
        )).ToListAsync(ct);
    }

}