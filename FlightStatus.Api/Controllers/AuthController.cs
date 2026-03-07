using FlightStatus.Api.Extensions;
using FlightStatus.Api.Models;
using FlightStatus.Application.UseCases.Auth.Commands.Login;
using FlightStatus.Domain.Abstractions.Primitive;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightStatus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _mediator;

    public AuthController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsFailure && result.Error.Code == "Unauthorized")
            return Unauthorized(result.ToApiResult("Вход выполнен"));
        return Ok(result.ToApiResult("Вход выполнен"));
    }
}
