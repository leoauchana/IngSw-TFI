using IngSw_Tfi.Domain.Entities;

namespace IngSw_Tfi.Domain.Enums;

public enum EmergencyLevel
{
    CRITICAL = 0,
    EMERGENCY,
    URGENCY,
    URGENCY_MINOR,
    WITHOUT_URGENCY
}

public static class EmergencyLevelExtensions
{
    public static Level ObtenerNivel(this EmergencyLevel tipo)
    {
        return tipo switch
        {
            EmergencyLevel.CRITICAL => new Level { Priority = 0, Name = "Crítica", MaximumDuration = TimeSpan.FromMinutes(5) },
            EmergencyLevel.EMERGENCY => new Level { Priority = 1, Name = "Emergencia", MaximumDuration = TimeSpan.FromMinutes(30) },
            EmergencyLevel.URGENCY => new Level { Priority = 2, Name = "Urgencia", MaximumDuration = TimeSpan.FromMinutes(60) },
            EmergencyLevel.URGENCY_MINOR => new Level { Priority = 3, Name = "Urgencia menor", MaximumDuration = TimeSpan.FromHours(2) },
            EmergencyLevel.WITHOUT_URGENCY => new Level { Priority = 4, Name = " Sin urgencia", MaximumDuration = TimeSpan.FromHours(4) },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
