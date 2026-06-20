using library_management.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
}