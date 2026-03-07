using FlightStatus.Api.Models;
using FlightStatus.Domain.Abstractions.Primitive;
using MediatR;

namespace FlightStatus.Api.Extensions;

public static class ResultExtensions
{
    public static ApiResult ToApiResult(this Result<Unit> result, string title, string? message = null) =>
        result.IsSuccess
            ? ApiResult.SuccessResult(title, message)
            : ApiResult.FailureResult(result.Error.Message ?? result.Error.Code, message);

    public static ApiResult<T> ToApiResult<T>(this Result<T> result, string? title = null) =>
        result.IsSuccess
            ? ApiResult<T>.SuccessResult(result.Value!, title)
            : ApiResult<T>.FailureResult(result.Error.Message ?? result.Error.Code);
}
