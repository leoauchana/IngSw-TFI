using IngSw_Tfi.Domain.Common;

namespace IngSw_Tfi.Domain.Entities;

public class User : EntityBase
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
