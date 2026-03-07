namespace FlightStatus.Application;

public class ApplicationNotFoundException : ApplicationLayerException
{
    public ApplicationNotFoundException(string? message = null, Exception? inner = null) : base(message, inner) { }
}
