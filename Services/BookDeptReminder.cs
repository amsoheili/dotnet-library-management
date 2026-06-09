using library_management.Constants;
using library_management.Data;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

public class BookDeptReminder : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BookDeptReminder> _logger;

    public BookDeptReminder(
        IServiceScopeFactory scopeFactory,
        ILogger<BookDeptReminder> logger
        )
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("checking the borrowed list");
            await ProcessAsync(cancellationToken);
            await Task.Delay(20 * 1000, cancellationToken);
        }
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var threshold = DateTime.UtcNow - TimeSpan.FromDays(AppConstants.MaxBorrowDays);
            var passedDateBorrowedBooks = await db.BorrowedBooks
                .Include(bB => bB.Book)
                .Include(bB => bB.Library)
                .Include(bB => bB.Member)
                .Where(bB => bB.Date < threshold)
                .ToListAsync(cancellationToken);
            foreach (var bB in passedDateBorrowedBooks)
            {
                _logger.LogWarning($"Library: {bB.Library.FullName}, Book: {bB.Book.Title}, Member: {bB.Member.FirstName} {bB.Member.LastName}");
            }
        }
        catch (Exception error)
        {
            _logger.LogWarning(error.Message);
        }
    }
}