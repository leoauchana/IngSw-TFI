using IngSw_Tfi.Domain.Common;
using IngSw_Tfi.Domain.ValueObjects;

namespace IngSw_Tfi.Domain.Entities;

public class BloodPressure
{
    public FrecuencySystolic? FrecuencySystolic{ get; set; }
    public FrecuencyDiastolic? FrecuencyDiastolic{ get; set; }
    public BloodPressure(FrecuencySystolic systolic, FrecuencyDiastolic diastolic)
    {
        FrecuencySystolic = systolic;
        FrecuencyDiastolic = diastolic;
    }
    public override string ToString()
    {
        return $"{FrecuencySystolic!.Value}/{FrecuencyDiastolic!.Value}";
    }
}
