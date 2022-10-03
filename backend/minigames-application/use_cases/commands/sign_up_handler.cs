using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Minigames.Application.Common.Interfaces;
using Minigames.Application.Dto;
using Minigames.Core;

namespace Minigames.Application.UseCases.Commands;

public record SignUpRequest(string email, string username, string password, string passwordConfirmation) : IRequest<AccessDto>;

public class SignUpHandler : IRequestHandler<SignUpRequest, AccessDto>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<SignUpHandler> _logger;

    public SignUpHandler(IIdentityService identityService, ILogger<SignUpHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<AccessDto> Handle(SignUpRequest request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByEmailAsync(request.email);

        if (user != null)
        {
            throw new AppException("There is already a user registered with this email", HttpStatusCode.Conflict);
        }

        if (request.password != request.passwordConfirmation)
        {
            throw new AppException("Passwords do not match", HttpStatusCode.BadRequest);
        }

        user = await _identityService.CreateUserAsync(request.email, request.username, request.password, UserRoles.User);
        
        _logger.LogInformation("Created a new user ready for the game");

        var authorizationToken = await _identityService.AuthorizeAsync(user);

        return new AccessDto(user.Id, user.UserName, authorizationToken);
    }
}
