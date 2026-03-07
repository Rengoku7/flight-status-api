using FlightStatus.Api.Extensions;
using FlightStatus.Api.Models;
using FlightStatus.Application.UseCases.Flights.Commands.AddFlight;
using FlightStatus.Application.UseCases.Flights.Commands.UpdateFlightStatus;
using FlightStatus.Application.UseCases.Flights.Queries.GetFlights;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightStatus.Api.Controllers;

/// <summary>Рейсы</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FlightsController : ControllerBase
{
    private readonly ISender _mediator;

    public FlightsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ApiResult<List<FlightDto>>> GetFlights([FromQuery] string? origin, [FromQuery] string? destination)
    {
        var result = await _mediator.Send(new GetFlightsQuery { Origin = origin, Destination = destination });
        return result.ToApiResult();
    }

    [HttpPost]
    [Authorize(Roles = "Moderator")]
    public async Task<IActionResult> AddFlight([FromBody] AddFlightCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsFailure)
            return BadRequest(result.ToApiResult<FlightIdResult>());
        return Ok(result.ToApiResult());
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Moderator")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateFlightStatusCommand command)
    {
        command.FlightId = id;
        var result = await _mediator.Send(command);
        if (result.IsFailure)
            return result.Error.Code == "NotFound" ? NotFound(result.ToApiResult()) : BadRequest(result.ToApiResult());
        return Ok(result.ToApiResult("Статус обновлён"));
    }
}
