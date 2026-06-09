using library_management.Data;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

public interface IAuthorService
{
    public Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto author);

    public Task<List<AuthorDto>> GetAuthorsAsync();
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
    public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto author)
    {

        var newAuthor = new Author
        {
            FirstName = author.FirstName,
            LastName = author.LastName,
            NationalCode = author.NationalCode,
            PhoneNumber = author.PhoneNumber
        };
        await _context.Authors.AddAsync(newAuthor);
        await _context.SaveChangesAsync();
        return new AuthorDto(newAuthor.Id, newAuthor.FirstName + " " + newAuthor.LastName);
    }

    [OutputCache(Duration = 10)]
    public async Task<List<AuthorDto>> GetAuthorsAsync()
    {
        return await _context.Authors.Select(a => new AuthorDto(
            a.Id,
            $"{a.FirstName} {a.LastName}"
        )).ToListAsync();
    }

}