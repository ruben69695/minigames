using Microsoft.AspNetCore.Mvc;
using MediatR;
using Minigames.Application.UseCases.Commands;
using Microsoft.AspNetCore.Authorization;
using Minigames.Application.UseCases.Queries;
using System.Net;

namespace Minigames.Api.Controllers;

public class IdentityController : ApiControllerBase 
{
    private readonly ILogger<IdentityController> _logger;

    public IdentityController(ILogger<IdentityController> logger, IMediator mediator) : base(mediator)
    {
        _logger = logger;
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetUser()
    {
        return Ok(await Mediator.Send(new GetUserRequest()));
    }

    [HttpPost("login")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpPost("signup")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    public async Task<ActionResult> SignUp(SignUpRequest request)
    {
        return Ok(await Mediator.Send(request));
    }
}