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

public record LoginRequest(string email, string password) : IRequest<AccessDto>;

public class LoginHandler : IRequestHandler<LoginRequest, AccessDto>
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<LoginHandler> _logger;
    private readonly IConfiguration _config;

    public LoginHandler(UserManager<User> userManager, ILogger<LoginHandler> logger, IConfiguration config)
    {
        _userManager = userManager;
        _logger = logger;
        _config = config;
    }

    public async Task<AccessDto> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.password))
        {
            throw new AppException("The email or password are incorrect", HttpStatusCode.Forbidden);
        }

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Sid, user.Id),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var accessToken = Factories.GenerateJsonWebToken(_config["Jwt:Key"], _config["Jwt:Issuer"], _config["Jwt:Audience"], claims);

        return new AccessDto(user.Id, user.UserName, accessToken);
    }
}