using Microsoft.AspNetCore.Mvc;
using MediatR;
using Minigames.Application.UseCases.Commands;
using Microsoft.AspNetCore.Authorization;
using Minigames.Application.UseCases.Queries;
using System.Net;
using System.Security.Claims;
using Minigames.Application;

namespace Minigames.Api.Controllers;

[Authorize]
public class GameDataController : ApiControllerBase
{
    private readonly ILogger<GameDataController> _logger;
    private IHttpContextAccessor _contextAccessor;

    public GameDataController(ILogger<GameDataController> logger, IMediator mediator, IHttpContextAccessor contextAccessor) : base(mediator)
    {
        _logger = logger;
        _contextAccessor = contextAccessor;
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetUserGameData()
    {
        var user = _contextAccessor.HttpContext?.User;

        if (user == null)
        {
            throw new AppException("User not found", HttpStatusCode.NotFound);
        }

        var id = user.Claims.First(c => c.Type == ClaimTypes.Sid).Value;

        return Ok(await Mediator.Send(new GetUserGameDataRequest(id)));
    }

    [Authorize]
    [HttpPost("me")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> UpdateUserGameData(UpdateUserGameDataRequest request)
    {
        var user = _contextAccessor.HttpContext?.User;

        if (user == null)
        {
            throw new AppException("User not found", HttpStatusCode.NotFound);
        }

        var id = user.Claims.First(c => c.Type == ClaimTypes.Sid).Value;
        request.SetUserId(id);

        return Ok(await Mediator.Send(request));
    }
}