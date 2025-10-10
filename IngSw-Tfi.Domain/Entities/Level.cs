using IngSw_Tfi.Domain.Common;

namespace IngSw_Tfi.Domain.Entities;

public class Level : EntityBase
{
    public int Priority { get; set; }
    public string? Name { get; set; }
    public TimeOnly MaximumDuration { get; set; }
}
