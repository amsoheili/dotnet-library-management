using library_management.DTOs;
using library_management.Entities;
using library_management.Filters;
using library_management.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace library_management.Controllers;

[ApiController]
[Route("books")]
public class BooksController(
    ILogger<BooksController> _logger,
    IBooksDataService _booksDataService
) : ControllerBase
{

    [Authorize(Roles = nameof(UserRolesEnum.SuperAdmin))]
    [ServiceFilter(typeof(ExecutionTimeFilter))]
    [HttpGet]
    public async Task<List<BookDTO>> GetBooks([FromQuery] string? search, [FromQuery] PaginationDto? pagination, CancellationToken ct)
    {
        List<BookDTO> list = new List<BookDTO>([]);
        var retrievedList = await _booksDataService.GetBooks(search, pagination, ct);
        return retrievedList.Select(b => new BookDTO(
                b.Id,
                b.Title,
                b.AuthorId
            )).ToList();
    }

    [Authorize(Roles = nameof(UserRolesEnum.SuperAdmin))]
    [HttpPost]
    public async Task<Boolean> CreateBookAsync([FromBody] CreateBookDto book, CancellationToken ct)
    {
        bool result = false;
        await _booksDataService.AddBook(new Book
        {
            Title = book.Title,
            AuthorId = book.AuthorId
        }, ct);
        return result;
    }

    [Authorize(Roles = nameof(UserRolesEnum.SuperAdmin))]
    [HttpPut("{id:guid}")]
    public async Task<ApiGeneralResponse<CreateBookDto>> UpdateBook([FromRoute] string id, [FromBody] CreateBookDto book, CancellationToken ct)
    {
        ApiGeneralResponse<CreateBookDto> result = new();
        var updatedBook = new Book
        {
            Title = book.Title,
            AuthorId = book.AuthorId
        };
        try
        {
            await _booksDataService.UpdateBook(id, updatedBook, ct);
            result.Result = book;
        }
        catch
        {
            result.error = true;
        }
        return result;
    }

    [Authorize(Roles = nameof(UserRolesEnum.SuperAdmin))]
    [HttpGet("{id:guid}")]
    public async Task<ApiGeneralResponse<BookDTO>> GetBook([FromRoute] string id, CancellationToken ct)
    {
        ApiGeneralResponse<BookDTO> result = new();
        var retrievedBook = await _booksDataService.GetBookById(id, ct);
        if (retrievedBook is null)
        {
            result.Success = false;
        }
        else
        {
            result.Result = new BookDTO
            (
                retrievedBook.Id,
                retrievedBook.Title,
                retrievedBook.AuthorId
            );
        }
        return result;
    }

    [Authorize(Roles = nameof(UserRolesEnum.SuperAdmin))]
    [HttpDelete("{id:guid}")]
    public async Task<ApiGeneralResponse<bool>> DeleteBook([FromRoute] string id, CancellationToken ct)
    {
        ApiGeneralResponse<bool> result = new();
        await _booksDataService.DeleteBook(id, ct);
        return result;
    }

    [Authorize(Roles = nameof(UserRolesEnum.SuperAdmin))]
    [HttpPost("count-books")]
    public async Task<ApiGeneralResponse<int>> CountBooks([FromQuery] string regex, CancellationToken ct)
    {
        return new ApiGeneralResponse<int> { Result = await _booksDataService.CountBooks(regex, ct) };
    }
}