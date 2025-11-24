namespace IngSw_Tfi.Domain.ValueObjects;

public class FrecuencyCardiac : Frecuency
{
    public FrecuencyCardiac(float value) : base(value)
    {
        if (value < 60 || value > 100)
            throw new ArgumentException("La frecuencia cardíaca debe estar entre 60 y 100 bpm.");
    }
}
