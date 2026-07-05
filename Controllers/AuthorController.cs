using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("authors")]
public class AuthorController
{
    private readonly IAuthorService _authorService;
    public AuthorController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [Authorize(Roles = nameof(UserRolesEnum.SuperAdmin))]
    [HttpPost]
    public async Task<ApiGeneralResponse<AuthorDto>> CreateAuthor([FromBody] CreateAuthorDto author, CancellationToken ct)
    {
        return new ApiGeneralResponse<AuthorDto> { Result = await _authorService.CreateAuthorAsync(author, ct) };
    }

    [Authorize(Roles = nameof(UserRolesEnum.SuperAdmin))]
    [HttpGet]
    public async Task<ApiGeneralResponse<List<AuthorDto>>> GetAuthors([FromQuery] PaginationDto? pagination, CancellationToken ct)
    {
        return new ApiGeneralResponse<List<AuthorDto>> { Result = await _authorService.GetAuthorsAsync(pagination, ct) };
    }
}