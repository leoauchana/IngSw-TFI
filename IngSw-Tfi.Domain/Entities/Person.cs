using IngSw_Tfi.Domain.Common;

namespace IngSw_Tfi.Domain.Entities;

public class Person : EntityBase
{
    //TODO: Podriamos implementar un value object para la propiedad cuil
    public string? Cuil { get; set; }
    public string? Name { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
}