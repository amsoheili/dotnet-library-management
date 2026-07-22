using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("subscription")]
public class SubscriptionController(
    IUserSubscriptionService _subscriptionService
)
{
    [Authorize(Roles = nameof(UserRolesEnum.User))]
    [HttpPost("{id}/purchase")]
    public async Task<ApiGeneralResponse<SubscriptionPurchaseResponseDto>> Purchase([FromRoute] string id, [FromBody] PurchaseSubscriptionPlanDto purchaseSubscriptionPlanDto, CancellationToken ct)
    {
        return new ApiGeneralResponse<SubscriptionPurchaseResponseDto> { Result = await _subscriptionService.Purchase(id, purchaseSubscriptionPlanDto, ct) };
    }

    // get request
    // purchase result of subscription
}