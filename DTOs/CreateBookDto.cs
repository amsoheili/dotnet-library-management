namespace library_management.DTOs;

public record CreateBookDto(
    string Title,
    string AuthorId
);