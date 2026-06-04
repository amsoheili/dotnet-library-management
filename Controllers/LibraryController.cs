using library_management.DTOs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("library")]
public class LibraryController : ControllerBase
{
    private readonly ILibraryService _libraryService;

    public LibraryController(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    [HttpPost("{libraryId}/add-book/{bookId}")]
    public async Task<ApiGeneralResponse<Boolean>> AddBook([FromRoute] string libraryId, [FromRoute] string bookId)
    {
        var result = await _libraryService.AddBookToLibraryAsync(libraryId, bookId);
        return new ApiGeneralResponse<bool> { Success = result };
    }

    [HttpGet("{id}/books")]
    public async Task<ApiGeneralResponse<List<BookDTO>>> GetLibraryBooks([FromRoute] string id)
    {
        return new ApiGeneralResponse<List<BookDTO>> { Result = await _libraryService.GetLibraryBooksAsync(id) };
    }

    [HttpPost]
    public async Task<ApiGeneralResponse<string>> CreateLibrary([FromBody] CreateLibraryDto library)
    {
        var libraryId = await _libraryService.CreateLibraryAsync(library.fullname);
        return new ApiGeneralResponse<string> { Result = libraryId };
    }

    [HttpGet]
    public async Task<ApiGeneralResponse<List<LibraryDto>>> GetAllLibraries()
    {
        return new ApiGeneralResponse<List<LibraryDto>> { Result = await _libraryService.GetAllLibrariesAsync() };
    }


}