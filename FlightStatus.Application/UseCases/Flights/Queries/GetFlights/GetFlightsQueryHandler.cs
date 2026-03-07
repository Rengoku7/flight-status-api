using FlightStatus.Application.Abstractions;
using FlightStatus.Application.Abstractions.Contracts;
using FlightStatus.Application.Flights;
using FlightStatus.Domain.Abstractions.Primitive;
using MediatR;

namespace FlightStatus.Application.UseCases.Flights.Queries.GetFlights;

/// <summary>Обработчик запроса списка рейсов</summary>
public class GetFlightsQueryHandler : IQueryHandler<GetFlightsQuery, PagedResult<FlightDto>>
{
    private readonly IFlightsRepository _repo;

    public GetFlightsQueryHandler(IFlightsRepository repo)
    {
        _repo = repo;
    }

    public async Task<Result<PagedResult<FlightDto>>> Handle(GetFlightsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repo.GetFlightsPagedAsync(
            request.Origin, request.Destination, request.Page, request.PageSize, cancellationToken);
        var dtos = items.Select(f => new FlightDto
        {
            Id = f.Id,
            Origin = f.Origin,
            Destination = f.Destination,
            Departure = f.Departure,
            Arrival = f.Arrival,
            Status = f.Status.ToString()
        }).ToList();
        return Result<PagedResult<FlightDto>>.Success(new PagedResult<FlightDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }
}
