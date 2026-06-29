using library_management.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("libraries")]
public class LibraryController : ControllerBase
{
    private readonly ILibraryService _libraryService;

    public LibraryController(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    [Authorize(Roles = nameof(UserRolesEnum.Admin))]
    [HttpPost("{libraryId:guid}/add-book/{bookId:guid}")]
    public async Task<ApiGeneralResponse<Boolean>> AddBook([FromRoute] string libraryId, [FromRoute] string bookId, CancellationToken ct)
    {
        var result = await _libraryService.AddBookToLibraryAsync(libraryId, bookId, ct);
        return new ApiGeneralResponse<bool> { Success = result };
    }

    [HttpGet("{id:guid}/books")]
    public async Task<ApiGeneralResponse<List<BookDTO>>> GetLibraryBooks([FromRoute] string id, [FromQuery] PaginationDto pagination, CancellationToken ct)
    {
        return new ApiGeneralResponse<List<BookDTO>> { Result = await _libraryService.GetLibraryBooksAsync(id, pagination, ct) };
    }

    [Authorize(Roles = nameof(UserRolesEnum.Admin))]
    [HttpPost]
    public async Task<ApiGeneralResponse<string>> CreateLibrary([FromBody] CreateLibraryDto library, CancellationToken ct)
    {
        var libraryId = await _libraryService.CreateLibraryAsync(library.fullname, ct);
        return new ApiGeneralResponse<string> { Result = libraryId };
    }

    [HttpGet]
    public async Task<ApiGeneralResponse<List<LibraryDto>>> GetAllLibraries([FromQuery] PaginationDto? pagination, CancellationToken ct)
    {
        return new ApiGeneralResponse<List<LibraryDto>> { Result = await _libraryService.GetAllLibrariesAsync(ct) };
    }

    [Authorize(Roles = nameof(UserRolesEnum.Admin))]
    [HttpPost("{id:guid}/add-member")]
    public async Task<ApiGeneralResponse<bool>> AddMember([FromRoute] string id, [FromBody] MemberDto member, CancellationToken ct)
    {
        return new ApiGeneralResponse<bool> { Result = await _libraryService.AddMember(id, member, ct) };
    }

    [HttpGet("{id:guid}/members")]
    public async Task<ApiGeneralResponse<List<MemberDto>>> GetMembers([FromRoute] string id, [FromQuery] PaginationDto pagination, CancellationToken ct)
    {
        return new ApiGeneralResponse<List<MemberDto>> { Result = await _libraryService.GetMembers(id, pagination, ct) };
    }

    [Authorize(Roles = nameof(UserRolesEnum.Admin))]
    [HttpPost("{id:guid}/lend-book")]
    public async Task<ApiGeneralResponse<bool>> LendBook([FromRoute] string id, [FromBody] LendBookDto lendBookData, CancellationToken ct)
    {
        return new ApiGeneralResponse<bool> { Result = await _libraryService.LendBook(id, lendBookData.bookId, lendBookData.memberId, ct) };
    }

    [Authorize(Roles = nameof(UserRolesEnum.Admin))]
    [HttpDelete("{id:guid}/retake-book")]
    public async Task<ApiGeneralResponse<bool>> RetakeBook([FromRoute] string id, [FromBody] RetakeBookDto retakeBookData, CancellationToken ct)
    {
        return new ApiGeneralResponse<bool> { Result = await _libraryService.RetakeBook(id, retakeBookData.bookId, retakeBookData.memberId, ct) };
    }

}