using System.Security.Cryptography.X509Certificates;
using library_management.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public interface IUserService
{
    public Task<string> Register(RegisterUserDto registerUserDto);

    public Task<LoginUserResponseDto> Login(LoginUserRequestDto loginUserRequestDto);

    // public Task<LoginUserResponseDto> RefreshToken(LoginUserRequestDto loginUserRequestDto);
}

public class UserService(
    AppDbContext _context,
    IPasswordHasher<User> _passwordHasher,
    ITokenService _tokenService,
    IConfiguration _config
) : IUserService
{
    public async Task<string> Register(RegisterUserDto registerUserDto)
    {
        var user = await _context.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Username == registerUserDto.username);

        if (user is not null)
        {
            return user.Id;
        }

        var newUser = new User
        {
            NationalCode = registerUserDto.nationalCode,
            PhoneNumber = registerUserDto.phoneNumber,
            Username = registerUserDto.username,
        };

        newUser.HashedPassword = _passwordHasher.HashPassword(newUser, registerUserDto.password);

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
        return newUser.Id;
    }

    public async Task<LoginUserResponseDto> Login(LoginUserRequestDto loginUserRequestDto)
    {
        var user = await _context.Users
                        .AsNoTracking()
                        .SingleOrDefaultAsync(u => u.Username == loginUserRequestDto.username);

        if (user is null)
            return null;

        var hashCompareResult = _passwordHasher.VerifyHashedPassword(
                                    user,
                                    user.HashedPassword,
                                    loginUserRequestDto.password
                                );

        if (hashCompareResult != PasswordVerificationResult.Success)
            return null;

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenRecord = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(int.Parse(_config.GetSection("jwt")["RefreshTokenExpirationDays"]))
        };

        await _context.RefreshTokens.AddAsync(refreshTokenRecord);
        await _context.SaveChangesAsync();

        return new LoginUserResponseDto(
            accessToken,
            refreshToken,
            new DateTimeOffset(refreshTokenRecord.ExpiryDate).ToUnixTimeMilliseconds()
            );
    }
}