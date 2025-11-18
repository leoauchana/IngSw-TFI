namespace IngSw_Tfi.Domain.ValueObjects;

public class Frecuency
{
    public double Value { get; set; }
    public Frecuency(double value)
    {
        Value = value;
    }
    public static Frecuency Create(double frecuency)
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
