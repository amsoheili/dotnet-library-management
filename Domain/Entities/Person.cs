using library_management.Entities;

public class Person : BaseEntity
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string NationalCode { get; set; }
    public string PhoneNumber { get; set; }
    public string? Address { get; set; }
    public List<PersonRole> Roles { get; set; }
    public PersonWallet Wallet { get; set; }

    public Person()
    {
        Wallet = new PersonWallet
        {
            Balance = 0.0000m,
            PersonId = Id,
        };
    }
}