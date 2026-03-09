using MediatR;
using Microsoft.Extensions.Logging;

namespace FlightStatus.Application.Behaviours;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILoggerFactory _loggerFactory;

    public LoggingBehaviour(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var logger = _loggerFactory.CreateLogger("FlightStatus.MediatR");
        var name = typeof(TRequest).Name;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var response = await next();
            sw.Stop();
            logger.LogInformation("{Request} выполнен за {ElapsedMs} мс", name, sw.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex, "{Request} ошибка за {ElapsedMs} мс", name, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
