using MediatR;
using FlightStatus.Domain.Abstractions.Primitive;

namespace FlightStatus.Application.Abstractions.Contracts;

public interface IQuery<TResult> : IRequest<Result<TResult>> { }

public interface IQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, Result<TResult>>
    where TQuery : IQuery<TResult> { }
