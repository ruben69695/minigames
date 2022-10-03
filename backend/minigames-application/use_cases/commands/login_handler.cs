using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Minigames.Application.Common.Interfaces;
using Minigames.Application.Dto;

namespace Minigames.Application.UseCases.Commands;

public record LoginRequest(string email, string password) : IRequest<AccessDto>;

public class LoginHandler : IRequestHandler<LoginRequest, AccessDto>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(IIdentityService identityService, ILogger<LoginHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<AccessDto> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByEmailAsync(request.email);

        if (user is null || !await _identityService.CheckUserPasswordAsync(user, request.password))
        {
            throw new AppException("The email or password are incorrect", HttpStatusCode.Forbidden);
        }

        var authorizationToken = await _identityService.AuthorizeAsync(user);

        return new AccessDto(user.Id, user.UserName, authorizationToken);
    }
}