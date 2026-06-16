using library_management.Entities;

public class IdempotencyRecord : BaseEntity
{
    public string Key { get; set; }
    public int StatusCode { get; set; }
    public string Body { get; set; }
}