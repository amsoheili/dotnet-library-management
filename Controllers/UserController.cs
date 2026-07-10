using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

[ApiController]
[Route("user")]
public class UserController(
    IUserService _userService
) : ControllerBase
{

    [HttpPost("register")]
    public async Task<ApiGeneralResponse<string>> Register([FromBody] RegisterUserDto registerUserDto)
    {
        return new ApiGeneralResponse<string> { Result = await _userService.Register(registerUserDto) };
    }

    [HttpPost("login")]
    public async Task<ApiGeneralResponse<LoginUserResponseDto>> Login([FromBody] LoginUserRequestDto loginUserRequestDto)
    {
        return new ApiGeneralResponse<LoginUserResponseDto> { Result = await _userService.Login(loginUserRequestDto) };
    }

    [HttpPost("refresh")]
    public async Task<ApiGeneralResponse<LoginUserResponseDto>> Refresh([FromBody] RefreshUserRequestDto refreshUserRequestDto)
    {
        return new ApiGeneralResponse<LoginUserResponseDto> { Result = await _userService.RefreshToken(refreshUserRequestDto) };
    }

    [Authorize]
    [HttpGet("me")]
    public ApiGeneralResponse<UserGetMeDto> Me()
    {
        return new ApiGeneralResponse<UserGetMeDto> { Result = _userService.GetMe() };
    }

    [Authorize(Roles = nameof(UserRolesEnum.SuperAdmin))]
    [HttpPost("grant-admin")]
    public async Task<ApiGeneralResponse<bool>> GrantAdmin([FromBody] GrantAdminDto grantAdminDto)
    {
        return new ApiGeneralResponse<bool> { Result = await _userService.GrantAdmin(grantAdminDto) };
    }

    [Authorize(Roles = nameof(UserRolesEnum.Admin))]
    [HttpPost("increase-wallet-credit")]
    public async Task<ApiGeneralResponse<bool>> IncreaseWalletCredit([FromBody] AddWalletCreditDto addWalletCreditDto, CancellationToken ct)
    {
        return new ApiGeneralResponse<bool> { Result = await _userService.IncreaseWalletCredit(addWalletCreditDto, ct) };
    }
}