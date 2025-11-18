namespace IngSw_Tfi.Domain.ValueObjects;

public class FrecuencyRespiratory : Frecuency
{
    public FrecuencyRespiratory(double value) : base(value)
    {
        if (value < 6 || value > 60)
            throw new ArgumentException("La frecuencia respiratoria debe estar entre 6 y 60 rpm.");
    }
}
