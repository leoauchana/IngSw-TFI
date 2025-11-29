namespace IngSw_Tfi.Domain.ValueObjects;

public class Frecuency
{
    public float Value { get; set; }
    public Frecuency(float value)
    {
        Value = value;
    }
    public static Frecuency Create(float frecuency)
    {
        if (frecuency <= 0)
            throw new ArgumentException("La frecuencia debe ser mayor a cero.");

        return new Frecuency(frecuency);
    }
    public override bool Equals(object? obj)
    {
        if (obj is not Frecuency other)
            return false;

        return Value == other.Value && GetType() == other.GetType();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value, GetType());
    }

    public override string ToString()
    {
        return $"{Value} bpm";
    }
}
