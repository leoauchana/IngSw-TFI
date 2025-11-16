namespace IngSw_Tfi.Domain.Entities;

public class Patient : Person
{
    public Affiliate? Affiliate { get; set; }
    public Domicilie? Domicilie { get; set; }
}
