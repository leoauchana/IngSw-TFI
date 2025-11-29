namespace IngSw_Tfi.Domain.Entities;

public class Doctor : Employee
{
    public Doctor()
    {
        Id = Guid.NewGuid();
    }
}
