using System.Text.RegularExpressions;

namespace IngSw_Tfi.Domain.ValueObjects;
public class Email
{
    public string Value { get; }
    private Email(string value)
    {
        Value = value;
    }
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email no puede ser vacío.");

        // Validación simple de email
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("Email con formato inválido.");

        return new Email(email);
    }
}
