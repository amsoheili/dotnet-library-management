public record UserGetMeDto(
    string userId,
    string phoneNumber,
    List<string>? roles
);