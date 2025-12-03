using IngSw_Tfi.Domain.Common;

namespace IngSw_Tfi.Domain.Entities;

public class Attention : EntityBase
{
    public Attention()
    {
        Id = Guid.NewGuid();
    }
    public Doctor? Doctor { get; set; }
    public Income? Income { get; set; }
    public string? Report { get; set; }
}
