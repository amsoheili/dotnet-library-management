using library_management.Entities;

public class UserSubscription : BaseEntity
{
    public string LibraryUserId { get; set; }
    public LibraryUser LibraryUser { get; set; }

    public string LibrarySubscriptionId { get; set; }
    public LibrarySubscription LibrarySubscription { get; set; }

    public SubscriptionBillingPeriod BillingPeriod { get; set; }
    public UserSubscriptionStatus Status { get; set; }

    public DateTime StartAt { get; set; }
    public DateTime EndAt { get; set; }
    public bool AutoRenewal { get; set; }
}