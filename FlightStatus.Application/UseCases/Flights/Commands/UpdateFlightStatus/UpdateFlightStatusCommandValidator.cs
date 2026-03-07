using FluentValidation;

namespace FlightStatus.Application.UseCases.Flights.Commands.UpdateFlightStatus;

public class UpdateFlightStatusCommandValidator : AbstractValidator<UpdateFlightStatusCommand>
{
    public UpdateFlightStatusCommandValidator()
    {
        RuleFor(x => x.FlightId).GreaterThan(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}
