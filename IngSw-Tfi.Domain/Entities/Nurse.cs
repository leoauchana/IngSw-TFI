namespace IngSw_Tfi.Domain.Entities;

public class Nurse : Employee
{
    public Nurse()
    {
        Id = Guid.NewGuid();
    }
}
