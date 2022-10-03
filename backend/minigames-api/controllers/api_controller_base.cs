using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Minigames.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiControllerBase : ControllerBase
{
    private readonly IMediator _mediator;

    protected IMediator Mediator => _mediator;

    public ApiControllerBase(IMediator mediator)
    {
        _mediator = mediator;
    }
}