namespace FlightStatus.Domain;

public class DomainException : Exception
{
    public DomainException(string? message = null, Exception? inner = null) : base(message, inner) { }
}
