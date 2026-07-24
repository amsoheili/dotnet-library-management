using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("payment")]
public class PaymentController(
    IPaymentService _paymentService
)
{

    [Authorize(Roles = nameof(UserRolesEnum.User))]
    [HttpPost("verify-by-wallet/{invoiceId}")]
    public async Task<ApiGeneralResponse<bool>> VerifyInvoiceByWallet([FromRoute] string invoiceId, CancellationToken ct)
    {
        return new ApiGeneralResponse<bool> { Result = await _paymentService.VerifyByWallet(null, invoiceId, ct) };
    }


    [HttpGet("methods")]
    public ApiGeneralResponse<bool> GetPaymentMethods()
    {
        return new ApiGeneralResponse<bool> { Result = false };
    }
}