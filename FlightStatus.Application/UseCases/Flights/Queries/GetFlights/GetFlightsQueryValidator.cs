using FluentValidation;

namespace FlightStatus.Application.UseCases.Flights.Queries.GetFlights;

public class GetFlightsQueryValidator : AbstractValidator<GetFlightsQuery>
{
    public GetFlightsQueryValidator()
    {
        RuleFor(x => x.Origin)
            .MaximumLength(256)
            .When(x => !string.IsNullOrEmpty(x.Origin));

        RuleFor(x => x.Destination)
            .MaximumLength(256)
            .When(x => !string.IsNullOrEmpty(x.Destination));
    }
}
