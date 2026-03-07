namespace FlightStatus.Infrastructure;

public class InfrastructureException : Exception
{
    public InfrastructureException(string? message = null, Exception? inner = null) : base(message, inner) { }
}
