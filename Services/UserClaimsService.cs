using System.IdentityModel.Tokens.Jwt;

public interface IUserClaimsService
{
    public string? GetUserId();
    public string? GetPhoneNumber();
}

public class UserClaimsService(
    IHttpContextAccessor _httpContextAccessor
) : IUserClaimsService
{
    public string? GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    }
    public string? GetPhoneNumber()
    {
        return _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.PhoneNumber)?.Value;
    }

}