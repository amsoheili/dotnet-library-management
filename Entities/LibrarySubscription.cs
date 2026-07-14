using library_management.Entities;

public class LibrarySubscription : BaseEntity
{
    public string Name { get; set; }
    public decimal MonthlyCost { get; set; } = 00.00m;
    public decimal YearlyCost { get; set; } = 00.00m;

    public string LibraryId { get; set; }
    public Library Library { get; set; }
}