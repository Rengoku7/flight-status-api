using FluentValidation;

namespace FlightStatus.Application.UseCases.Flights.Queries.GetFlightById;

public class GetFlightByIdQueryValidator : AbstractValidator<GetFlightByIdQuery>
{
    public GetFlightByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
