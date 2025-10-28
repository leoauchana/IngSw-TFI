using IngSw_Tfi.Domain.Common;
using IngSw_Tfi.Domain.ValueObjects;

namespace IngSw_Tfi.Domain.Entities;

public class Person : EntityBase
{
    //TODO: Podriamos implementar un value object para la propiedad cuil
    public Cuil? Cuil { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public Domicilie? Domicilie { get; set; }
}