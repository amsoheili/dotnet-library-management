using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("me")]
    public ApiGeneralResponse<object> Me()
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var phoneNumber = User.FindFirst(JwtRegisteredClaimNames.PhoneNumber)?.Value;
        return new ApiGeneralResponse<object> { Result = new { userId, phoneNumber } };
    }
}