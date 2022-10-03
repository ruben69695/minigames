using Microsoft.AspNetCore.Identity;
using Minigames.Application.Common.Interfaces;
using Minigames.Core;

namespace Minigames.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly IUserClaimsFactory<User> _userClaimsFactory;
    private readonly IAuthorizationService _authorzationService;

    public IdentityService(UserManager<User> userManager, 
        IUserClaimsFactory<User> userClaimsFactory, 
        IAuthorizationService authorzationService)
    {
        _userManager = userManager;
        _userClaimsFactory = userClaimsFactory;
        _authorzationService = authorzationService;
    }

    public async Task<string> AuthorizeAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var claims = _userClaimsFactory.Create(user, roles);

        return _authorzationService.CreateAuthorization(claims);
    }

    public async Task<bool> CheckUserPasswordAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<User> CreateUserAsync(string email, string username, string password, params string[] roles)
    {
        var user = User.Create(email, username);

        var result = await _userManager.CreateAsync(user, password);
        await _userManager.AddToRolesAsync(user, roles);

        return user;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(User user)
    {
        return await _userManager.GetRolesAsync(user);
    }
}