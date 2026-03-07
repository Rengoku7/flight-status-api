namespace FlightStatus.Application;

public class ApplicationLayerException : Exception
{
    public ApplicationLayerException(string? message = null, Exception? inner = null) : base(message, inner) { }
}
