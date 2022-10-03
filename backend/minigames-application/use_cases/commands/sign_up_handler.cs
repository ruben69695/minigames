using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minigames.Application.Dto;
using Minigames.Application.Tools;
using Minigames.Core;

namespace Minigames.Application.UseCases.Commands;

public record SignUpRequest(string email, string username, string password, string passwordConfirmation) : IRequest<AccessDto>;

public class SignUpHandler : IRequestHandler<SignUpRequest, AccessDto>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<SignUpHandler> _logger;
    private readonly IConfiguration _config;

    public SignUpHandler(UserManager<User> userManager, ILogger<SignUpHandler> logger, IConfiguration config)
    {
        _userManager = userManager;
        _logger = logger;
        _config = config;
    }

    public async Task<AccessDto> Handle(SignUpRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.email);

        if (user != null)
        {
            throw new AppException("There is already a user registered with this email", HttpStatusCode.Conflict);
        }

        if (request.password != request.passwordConfirmation)
        {
            throw new AppException("Passwords do not match", HttpStatusCode.BadRequest);
        }

        user = User.Create(request.email, request.username);

        _logger.LogInformation("Creating new user for the game");

        await _userManager.CreateAsync(user, request.password);
        await _userManager.AddToRoleAsync(user, UserRoles.User);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Sid, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, UserRoles.User)
        };

        var accessToken = Factories.GenerateJsonWebToken(_config["Jwt:Key"], _config["Jwt:Issuer"], _config["Jwt:Audience"], claims);

        return new AccessDto(user.Id, user.UserName, accessToken);
    }
}
