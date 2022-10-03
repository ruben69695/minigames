using System.Security.Claims;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Minigames.Application.Dto;

namespace Minigames.Application.UseCases.Queries;

public record GetUserRequest() : IRequest<UserDto>;

[Obsolete]
public class GetUserHandler : IRequestHandler<GetUserRequest, UserDto>
{
    private readonly ILogger<GetUserHandler> _logger;
    private readonly IHttpContextAccessor _contextAccessor;


    public GetUserHandler(ILogger<GetUserHandler> logger, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _contextAccessor = contextAccessor;
    }

    public async Task<UserDto> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        var user = _contextAccessor.HttpContext?.User;

        if (user == null)
        {
            throw new AppException("User not found", HttpStatusCode.NotFound);
        }

        var id = user.Claims.First(c => c.Type == ClaimTypes.Sid).Value;
        var email = user.Claims.First(c => c.Type == ClaimTypes.Email).Value;

        return await Task.FromResult(new UserDto(id, email, user.Identity!.IsAuthenticated));
    }
}