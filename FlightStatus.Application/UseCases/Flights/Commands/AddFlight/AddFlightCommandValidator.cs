using FluentValidation;

namespace FlightStatus.Application.UseCases.Flights.Commands.AddFlight;

/// <summary>Валидация полей команды добавления рейса.</summary>
public class AddFlightCommandValidator : AbstractValidator<AddFlightCommand>
{
    public AddFlightCommandValidator()
    {
        RuleFor(x => x.Origin).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Destination).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Status).IsInEnum();
    }
}
