using library_management.Entities;

public class PersonRole : BaseEntity
{
    public string PersonId { get; set; }
    public Person Person { get; set; }
    public UserRolesEnum Role { get; set; }
}