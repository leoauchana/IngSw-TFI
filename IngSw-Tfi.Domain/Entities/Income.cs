using IngSw_Tfi.Domain.Common;
using IngSw_Tfi.Domain.Enums;
using IngSw_Tfi.Domain.ValueObjects;

namespace IngSw_Tfi.Domain.Entities;

public class Income : EntityBase
{
    public Income()
    {
        Id = Guid.NewGuid();
    }
    public Attention? Attention { get; set; }
    public Patient? Patient { get; set; }
    public Nurse? Nurse { get; set; }
    public EmergencyLevel EmergencyLevel { get; set; }
    public IncomeStatus? IncomeStatus { get; set; }
    public string? Report { get; set; }
    public DateTime IncomeDate { get; set; }
    public float Temperature { get; set; }
    public FrecuencyCardiac? FrecuencyCardiac { get; set; }
    public FrecuencyRespiratory? FrecuencyRespiratory { get; set; }
    public BloodPressure? BloodPressure { get; set; }
}
