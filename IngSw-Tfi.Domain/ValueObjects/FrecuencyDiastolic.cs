namespace IngSw_Tfi.Domain.ValueObjects;

public class FrecuencyDiastolic : Frecuency
{
    public FrecuencyDiastolic(float value) : base(value)
    {
        if (value < 60 || value > 80)
            throw new ArgumentException("La frecuencia diastólica debe estar entre 60 y 80 mmHg.");
    }
}
