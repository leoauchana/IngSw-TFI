using IngSw_Tfi.Domain.Common;
using IngSw_Tfi.Domain.ValueObjects;

namespace IngSw_Tfi.Domain.Entities;

public class Person : EntityBase
{
    public Cuil? Cuil { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }   
    public DateTime BirthDate { get; set; }
}