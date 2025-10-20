namespace IngSw_Tfi.Application.Exceptions;

public class EntityNotFoundException : ApplicationExceptions
{
    public EntityNotFoundException(string message) : base(message) { }
}
