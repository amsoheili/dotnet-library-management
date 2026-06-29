using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using library_management.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public interface IUserService
{
    public Task<string> Register(RegisterUserDto registerUserDto);

    public Task<LoginUserResponseDto> Login(LoginUserRequestDto loginUserRequestDto);

    public Task<LoginUserResponseDto> RefreshToken(RefreshUserRequestDto refreshUserRequestDto);

    public UserGetMeDto GetMe();
    public Task<bool> AssignRole(AssignRoleDto assignRoleDto);
}

public class UserService(
    AppDbContext _context,
    IPasswordHasher<LibraryUser> _passwordHasher,
    ITokenService _tokenService,
    IConfiguration _config,
    IUserClaimsService _userClaimsService
) : IUserService
{
    public async Task<string> Register(RegisterUserDto registerUserDto)
    {
        var user = await _context.LibraryUsers.AsNoTracking().SingleOrDefaultAsync(u => u.Username == registerUserDto.username);

        if (user is not null)
        {
            return user.Id;
        }

        var newUser = new LibraryUser
        {
            NationalCode = registerUserDto.nationalCode,
            PhoneNumber = registerUserDto.phoneNumber,
            Username = registerUserDto.username,
        };

        newUser.HashedPassword = _passwordHasher.HashPassword(newUser, registerUserDto.password);

        await _context.LibraryUsers.AddAsync(newUser);
        await _context.SaveChangesAsync();
        return newUser.Id;
    }

    public async Task<LoginUserResponseDto> Login(LoginUserRequestDto loginUserRequestDto)
    {
        var user = await _context.LibraryUsers
                        .AsNoTracking()
                        .Include(lu => lu.Roles)
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
        var userRoles = user.Roles.Select(r => r.Role).ToList();
        var accessToken = _tokenService.GenerateAccessToken(user, userRoles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenRecord = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiryDate = _tokenService.GetRefreshExpiryDate()
        };

        await _context.RefreshTokens.AddAsync(refreshTokenRecord);
        await _context.SaveChangesAsync();

        return new LoginUserResponseDto(
            accessToken,
            refreshToken,
            new DateTimeOffset(refreshTokenRecord.ExpiryDate).ToUnixTimeMilliseconds()
            );
    }


    public async Task<LoginUserResponseDto> RefreshToken(RefreshUserRequestDto refreshUserRequestDto)
    {
        var userId = _userClaimsService.GetUserId();

        var refreshTokenRecord = await _context.RefreshTokens
            .AsNoTracking()
            .Include(r => r.User)
            .ThenInclude(u => u.Roles)
            .SingleOrDefaultAsync(rt => rt.Token == refreshUserRequestDto.refreshToken && rt.UserId == userId);

        if (refreshTokenRecord is null || refreshTokenRecord.User is null)
            return null;

        if (refreshTokenRecord.ExpiryDate < DateTime.UtcNow)
            return null;

        var userRoles = refreshTokenRecord.User.Roles.Select(r => r.Role).ToList();
        var accessToken = _tokenService.GenerateAccessToken(refreshTokenRecord.User, userRoles);
        var refreshToken = _tokenService.GenerateRefreshToken();


        var newRefreshTokenRecord = new RefreshToken
        {
            Token = refreshToken,
            UserId = refreshTokenRecord.UserId,
            ExpiryDate = _tokenService.GetRefreshExpiryDate()
        };

        _context.RefreshTokens.Remove(refreshTokenRecord);
        _context.RefreshTokens.Add(newRefreshTokenRecord);

        _context.SaveChangesAsync();
        return new LoginUserResponseDto
        (
            accessToken,
            refreshToken,
            new DateTimeOffset(_tokenService.GetAccessExpiryDate()).ToUnixTimeMilliseconds()
        );
    }

    public UserGetMeDto GetMe()
    {
        return new UserGetMeDto(
            userId: _userClaimsService.GetUserId(),
            phoneNumber: _userClaimsService.GetPhoneNumber(),
            roles: _userClaimsService.GetRoles()
        );
    }

    public async Task<bool> AssignRole(AssignRoleDto assignRoleDto)
    {
        var alreadyHasRole = await _context.PersonRoles
                        .AsNoTracking()
                        .AnyAsync(pr => pr.PersonId == assignRoleDto.userId && pr.Role == assignRoleDto.role);

        if (alreadyHasRole)
            return true;

        _context.PersonRoles.Add(new PersonRole { PersonId = assignRoleDto.userId, Role = assignRoleDto.role });

        await _context.SaveChangesAsync();

        return true;
    }

}