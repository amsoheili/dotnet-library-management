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
    public async Task<ApiGeneralResponse<AuthorDto>> CreateAuthor([FromBody] CreateAuthorDto author)
    {
        return new ApiGeneralResponse<AuthorDto> { Result = await _authorService.CreateAuthorAsync(author) };
    }
}