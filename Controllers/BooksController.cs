using library_management.DTOs;
using library_management.Entities;
using library_management.Services;
using Microsoft.AspNetCore.Mvc;

namespace library_management.Controllers;

[ApiController]
[Route("books")]
public class BooksController : ControllerBase
{
    private readonly IBooksDataService _booksDataService;

    public BooksController(IBooksDataService booksDataService)
    {
        _booksDataService = booksDataService;
    }

    [HttpGet]
    public async Task<List<BookDTO>> GetBooks()
    {
        List<BookDTO> list = new List<BookDTO>([]);
        var retrievedList = await _booksDataService.GetBooks();
        return retrievedList.Select(b => new BookDTO(
                b.Id,
                b.Title,
                b.Author
            )).ToList();
    }

    [HttpPost]
    public async Task<Boolean> CreateBookAsync([FromBody] CreateBookDto book)
    {
        bool result = false;
        await _booksDataService.AddBook(new Book
        {
            Title = book.title,
            Author = book.author
        });
        return result;
    }

    [HttpPut("{id}")]
    public async Task<ApiGeneralResponse<CreateBookDto>> UpdateBook([FromRoute] string id, [FromBody] CreateBookDto book)
    {
        ApiGeneralResponse<CreateBookDto> result = new();
        var updatedBook = new Book
        {
            Title = book.title,
            Author = book.author
        };
        try
        {
            await _booksDataService.UpdateBook(id, updatedBook);
            result.Result = book;
        }
        catch
        {
            result.error = true;
        }
        return result;
    }

    [HttpGet("{id}")]
    public async Task<ApiGeneralResponse<BookDTO>> GetBook([FromRoute] string id)
    {
        ApiGeneralResponse<BookDTO> result = new();
        var retrievedBook = await _booksDataService.GetBookById(id);
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
                retrievedBook.Author
            );
        }
        return result;
    }

    [HttpDelete("{id}")]
    public async Task<ApiGeneralResponse<bool>> DeleteBook([FromRoute] string id)
    {
        ApiGeneralResponse<bool> result = new();
        await _booksDataService.DeleteBook(id);
        return result;
    }
}