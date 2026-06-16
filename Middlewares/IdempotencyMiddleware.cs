using System.Net;
using library_management.Data;
using Microsoft.EntityFrameworkCore;

public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IdempotencyMiddleware> _logger;

    public IdempotencyMiddleware(RequestDelegate next, ILogger<IdempotencyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        // check if i want to handle the endpoint
        if (!HttpMethods.IsPost(context.Request.Method))
        {
            _logger.LogInformation("Method was not post");
            await _next(context);
            return;
        }

        // get the idempotency id in the header
        if (!context.Request.Headers.TryGetValue(AppHeaders.Idempotency, out var key))
        {
            _logger.LogInformation("Request did not contain idempotency header");
            await _next(context);
            return;
        }

        var record = await db.IdempotencyRecords.FirstOrDefaultAsync(ir => ir.Key == key.ToString());

        // check if i have stored the request 
        if (record is not null)
        {
            _logger.LogInformation("Reading the record from idempotency table");
            context.Response.StatusCode = record.StatusCode;
            await context.Response.WriteAsync(record.Body);
            return;
        }

        _logger.LogInformation("The Idempotency table did not contain the id");
        var originalResponse = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await _next(context);

        memoryStream.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(memoryStream).ReadToEndAsync();

        await db.IdempotencyRecords.AddAsync(new IdempotencyRecord
        {
            Key = key.ToString(),
            Body = body,
            StatusCode = context.Response.StatusCode
        });

        await db.SaveChangesAsync();

        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalResponse);
    }
}




// public class IdempotencyMiddleware
// {
//     private readonly RequestDelegate _next;

//     public IdempotencyMiddleware(RequestDelegate next)
//     {
//         _next = next;
//     }

//     public async Task Invoke(HttpContext context, AppDbContext db)
//     {
//         var key = context.Request.Headers["Idempotency-Key"].FirstOrDefault();

//         if (string.IsNullOrEmpty(key))
//         {
//             await _next(context);
//             return;
//         }

//         var record = await db.IdempotencyRecords.FindAsync(key);

//         if (record != null)
//         {
//             context.Response.StatusCode = record.StatusCode;
//             await context.Response.WriteAsync(record.Response);
//             return;
//         }

//         var originalBody = context.Response.Body;
//         using var newBody = new MemoryStream();
//         context.Response.Body = newBody;

//         await _next(context);

//         newBody.Seek(0, SeekOrigin.Begin);
//         var responseText = await new StreamReader(newBody).ReadToEndAsync();

//         db.IdempotencyRecords.Add(new IdempotencyRecord
//         {
//             Key = key,
//             Response = responseText,
//             StatusCode = context.Response.StatusCode
//         });

//         await db.SaveChangesAsync();

//         newBody.Seek(0, SeekOrigin.Begin);
//         await newBody.CopyToAsync(originalBody);
//     }
// }
