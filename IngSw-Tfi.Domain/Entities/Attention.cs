using IngSw_Tfi.Domain.Common;

namespace IngSw_Tfi.Domain.Entities;

public class Attention : EntityBase
{
    public Doctor? Doctor { get; set; }
    public string? Report { get; set; }
}
