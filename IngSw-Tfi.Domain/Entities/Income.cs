using IngSw_Tfi.Domain.Common;
using IngSw_Tfi.Domain.Enums;

namespace IngSw_Tfi.Domain.Entities;

public class Income : EntityBase
{
    public Attention? Attention { get; set; }
    public Patient? Patient { get; set; }
    public Nurse? Nurse { get; set; }
    public EmergencyLevel? EmergencyLevel { get; set; }
    public IncomeStatus? IncomeStatus { get; set; }
    public string? Description { get; set; }
    public DateTime? IncomeDate { get; set; }

    // Signos vitales / métricas
    public float? Temperature { get; set; }
    public float? FrequencyCardiac { get; set; }
    public float? FrequencyRespiratory { get; set; }
    public float? SystolicRate { get; set; }
    public float? DiastolicRate { get; set; }

    // TODO: Completar propiedades y determinar si aplicar patron value object.
}
