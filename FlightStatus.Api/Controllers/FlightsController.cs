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

    /// <summary>Список рейсов. Сортировка по времени прилёта. Фильтр по пункту вылета и/или назначения — опционально.</summary>
    /// <param name="origin">Пункт вылета (опционально).</param>
    /// <param name="destination">Пункт назначения (опционально).</param>
    /// <response code="200">Список рейсов в data.</response>
    [HttpGet]
    public async Task<ApiResult<List<FlightDto>>> GetFlights([FromQuery] string? origin, [FromQuery] string? destination)
    {
        var result = await _mediator.Send(new GetFlightsQuery { Origin = origin, Destination = destination });
        return result.ToApiResult();
    }

    /// <summary>Добавить рейс. Только роль Moderator.</summary>
    /// <param name="command">Origin, Destination, Departure, Arrival, Status.</param>
    /// <response code="200">Рейс создан, в data — id.</response>
    /// <response code="400">Ошибка валидации.</response>
    [HttpPost]
    [Authorize(Roles = "Moderator")]
    public async Task<IActionResult> AddFlight([FromBody] AddFlightCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsFailure)
            return BadRequest(result.ToApiResult<FlightIdResult>());
        return Ok(result.ToApiResult());
    }

    /// <summary>Изменить статус рейса. Только роль Moderator. В теле только Status (InTime, Delayed, Cancelled).</summary>
    /// <param name="id">Id рейса.</param>
    /// <param name="command">Объект с полем Status.</param>
    /// <response code="200">Статус обновлён.</response>
    /// <response code="404">Рейс не найден.</response>
    /// <response code="400">Ошибка валидации.</response>
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Moderator")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateFlightStatusCommand command)
    {
        command.FlightId = id;
        var result = await _mediator.Send(command);
        return Ok(result.ToApiResult("Статус обновлён"));
    }
}
