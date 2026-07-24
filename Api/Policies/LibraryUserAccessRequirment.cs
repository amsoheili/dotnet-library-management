using library_management.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

public class LibraryUserAccessRequirment : IAuthorizationRequirement { }

public class LibraryUserAccessHandler(
    IUserClaimsService _userClaimService,
    AppDbContext _db
) : AuthorizationHandler<LibraryUserAccessRequirment, string>
{
    protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            LibraryUserAccessRequirment requirement,
            string requestedLibraryId)
    {
        var userId = _userClaimService.GetUserId();

        var isLibraryUser = await _db.LibraryUsers.AsNoTracking().AnyAsync(u => u.Id == userId && u.LibraryId == requestedLibraryId);

        var isSuperAdmin = await _db.PersonRoles.AsNoTracking().AnyAsync(pr => pr.PersonId == userId && pr.Role == UserRolesEnum.SuperAdmin);

        if (isLibraryUser || isSuperAdmin)
        {
            context.Succeed(requirement);
        }
        return;
    }
}