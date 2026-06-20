public record RegisterUserDto(
    string username,
    string password,
    string nationalCode,
    string phoneNumber
);

public record LoginUserRequestDto(
    string username,
    string password
);

public record LoginUserResponseDto(
    string accessToken,
    string refreshToken,
    long expirationDate
);