using library_management.Data;

public interface IAuthorService
{
    public Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto author);
}

public class AuthorService : IAuthorService
{
    private AppDbContext _context;

    public AuthorService(AppDbContext context)
    {
        _context = context;
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
}