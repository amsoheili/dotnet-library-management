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
    public async Task<List<BookDTO>> GetBooksAsync()
    {
        List<BookDTO> list = new List<BookDTO>([]);
        await Task.Run(() =>
        {
            list = _booksDataService.GetBooks().Select(b => new BookDTO(
                b.Id,
                b.Title,
                b.Author
                )).ToList();
        });
        return list;
    }

    [HttpPost]
    public async Task<Boolean> CreateBookAsync([FromBody] CreateBookDto book)
    {
        bool result = false;
        await Task.Run(() =>
        {
            try
            {
                _booksDataService.AddBook(new Book
                {
                    Title = book.title,
                    Author = book.author
                });
                result = true;
            }
            catch
            {
                result = false;
            }
        });
        return result;
    }

    [HttpPut("{id}")]
    public async Task<BookDTO> updateBook([FromRoute] string id, [FromBody] CreateBookDto book)
    {

    }
}