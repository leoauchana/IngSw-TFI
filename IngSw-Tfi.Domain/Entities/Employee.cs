namespace IngSw_Tfi.Domain.Entities;

public class Employee : Person
{
    public string? Registration { get; set; }
    public string? PhoneNumber { get; set; }
    public User? User { get; set; }
}
