using IngSw_Tfi.Domain.Common;

namespace IngSw_Tfi.Domain.Entities;

public class Domicilie : EntityBase
{
    public int Number { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
}
