using FlightStatus.Api.Models;
using FlightStatus.Application;
using FlightStatus.Domain;
using FlightStatus.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FlightStatus.Api.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var ex = context.Exception.GetBaseException();
        var (statusCode, result) = ex switch
        {
            DomainException => (400, ApiResult.FailureResult("Ошибка домена", ex.Message)),
            ApplicationLayerException => (400, ApiResult.FailureResult("Ошибка приложения", ex.Message)),
            InfrastructureException => (500, ApiResult.FailureResult("Техническая ошибка", ex.Message)),
            _ => (500, ApiResult.FailureResult("Ошибка сервера", "Произошла непредвиденная ошибка."))
        };

        _logger.LogWarning(ex, "Исключение слоя: {Type}, код {Code}", ex.GetType().Name, statusCode);
        context.Result = new ObjectResult(result) { StatusCode = statusCode };
        context.ExceptionHandled = true;
    }
}
