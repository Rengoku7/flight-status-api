using MediatR;
using FlightStatus.Domain.Abstractions.Primitive;

namespace FlightStatus.Application.Abstractions.Contracts;

public interface ICommand : IRequest<Result<Unit>> { }

public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result<Unit>>
    where TCommand : ICommand { }

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse> { }
