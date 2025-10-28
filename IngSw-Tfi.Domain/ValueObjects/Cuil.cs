using System.Text.RegularExpressions;

namespace IngSw_Tfi.Domain.ValueObjects;

public class Cuil
{
    public string Value { get; }
    private Cuil(string value)
    {
        Value = value;
    }
    public static Cuil Create(string cuil)
    {
        if (string.IsNullOrWhiteSpace(cuil))
            throw new ArgumentException("CUIL no puede ser vacío.");

        // Validación simple de formato: 2 dígitos - 8 dígitos - 1 dígito
        if (!Regex.IsMatch(cuil, @"^\d{2}-\d{8}-\d$"))
            throw new ArgumentException("CUIL con formato inválido.");

        return new Cuil(cuil);
    }
}
