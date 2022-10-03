using Microsoft.AspNetCore.Mvc;
using MediatR;
using Minigames.Application.UseCases.Commands;
using Microsoft.AspNetCore.Authorization;
using Minigames.Application.UseCases.Queries;
using System.Net;

namespace Minigames.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class IdentityController : ControllerBase
{
    private readonly ILogger<IdentityController> _logger;
    private readonly IMediator _mediator;

    public IdentityController(ILogger<IdentityController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetUser()
    {
        return Ok(await _mediator.Send(new GetUserRequest()));
    }

    [HttpPost("login")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        return Ok(await _mediator.Send(request));
    }

    [HttpPost("signup")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    public async Task<ActionResult> SignUp(SignUpRequest request)
    {
        return Ok(await _mediator.Send(request));
    }
}