using FluentValidation;

namespace FlightStatus.Application.UseCases.Flights.Commands.UpdateFlightStatus;

/// <summary>Валидация id рейса и значения статуса.</summary>
public class UpdateFlightStatusCommandValidator : AbstractValidator<UpdateFlightStatusCommand>
{
    public UpdateFlightStatusCommandValidator()
    {
        RuleFor(x => x.FlightId).GreaterThan(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}
