namespace IngSw_Tfi.Domain.ValueObjects;

public class FrecuencyCardiac : Frecuency
{
    public FrecuencyCardiac(double value) : base(value)
    {
        if (value < 30 || value > 220)
            throw new ArgumentException("La frecuencia cardíaca debe estar entre 30 y 220 bpm.");
    }
}
