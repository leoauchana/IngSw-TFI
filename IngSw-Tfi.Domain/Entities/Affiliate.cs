using IngSw_Tfi.Domain.Common;

namespace IngSw_Tfi.Domain.Entities;

public class Affiliate : EntityBase
{
    public string? AffiliateNumber { get; set; }
    public SocialWork? SocialWork { get; set; }
}
