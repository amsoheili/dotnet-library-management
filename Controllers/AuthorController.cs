using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("author")]
public class AuthorController
{
    private readonly IAuthorService _authorService;
    public AuthorController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpPost]
    public async Task<ApiGeneralResponse<AuthorDto>> CreateAuthor([FromBody] CreateAuthorDto author, CancellationToken ct)
    {
        return new ApiGeneralResponse<AuthorDto> { Result = await _authorService.CreateAuthorAsync(author, ct) };
    }

    [HttpGet]
    public async Task<ApiGeneralResponse<List<AuthorDto>>> GetAuthors(CancellationToken ct)
    {
        return new ApiGeneralResponse<List<AuthorDto>> { Result = await _authorService.GetAuthorsAsync(ct) };
    }
}