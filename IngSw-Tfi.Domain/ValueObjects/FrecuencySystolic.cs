namespace IngSw_Tfi.Domain.ValueObjects;

public class FrecuencySystolic : Frecuency
{
    public FrecuencySystolic(float value) : base(value)
    {
        if (value < 90 || value > 120)
            throw new ArgumentException("La frecuencia sistólica debe estar entre 90 y 120 mmHg.");
    }
}
