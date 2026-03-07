using FlightStatus.Application.Abstractions.Contracts;
using FlightStatus.Application.Flights;
using FlightStatus.Domain.Abstractions.Primitive;
using MediatR;

namespace FlightStatus.Application.UseCases.Flights.Queries.GetFlights;

/// <summary>Обработчик запроса списка рейсов</summary>
public class GetFlightsQueryHandler : IQueryHandler<GetFlightsQuery, List<FlightDto>>
{
    private readonly IFlightsRepository _repo;

    public GetFlightsQueryHandler(IFlightsRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<List<FlightDto>>> Handle(GetFlightsQuery request, CancellationToken cancellationToken)
    {
        var flights = await _repo.GetFlightsAsync(request.Origin, request.Destination, cancellationToken);
        var dtos = flights.Select(f => new FlightDto
        {
            Id = f.Id,
            Origin = f.Origin,
            Destination = f.Destination,
            Departure = f.Departure,
            Arrival = f.Arrival,
            Status = f.Status.ToString()
        }).ToList();
        return Result<List<FlightDto>>.Success(dtos);
    }
}
