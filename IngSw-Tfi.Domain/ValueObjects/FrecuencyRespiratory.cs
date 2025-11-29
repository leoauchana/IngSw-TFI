namespace IngSw_Tfi.Domain.ValueObjects;

public class FrecuencyRespiratory : Frecuency
{
    public FrecuencyRespiratory(float value) : base(value)
    {
        if (value < 12 || value > 20)
            throw new ArgumentException("La frecuencia respiratoria debe estar entre 12 y 20 rpm.");
    }
}
