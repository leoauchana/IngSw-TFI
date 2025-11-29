using IngSw_Tfi.Domain.Enums;

namespace IngSw_Tfi.Domain.ValueObjects;

public readonly record struct IncomePriority : IComparable<IncomePriority>
{
    private EmergencyLevel EmergencyLevel { get; }
    private DateTime IncomeDate { get; }
    public IncomePriority(EmergencyLevel emergencyLevel, DateTime incomeDate)
    {
        EmergencyLevel = emergencyLevel;
        IncomeDate = incomeDate;
    }
    public int CompareTo(IncomePriority other)
    {
        // 1) Prioridad principal: nivel de emergencia (menor sale primero)
        int cmp = EmergencyLevel.CompareTo(other.EmergencyLevel);
        if (cmp != 0) return cmp;

        // 2) Si tienen misma emergencia → más antiguo sale primero
        return IncomeDate.CompareTo(other.IncomeDate);
    }

}



