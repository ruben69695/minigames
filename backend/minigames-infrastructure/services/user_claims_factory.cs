using System.Security.Claims;
using Minigames.Application.Common.Interfaces;
using Minigames.Core;

namespace Minigames.Infrastructure.Services;

public class UserClaimsFactory : IUserClaimsFactory<User>
{
    public IEnumerable<Claim> Create(User user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Sid, user.Id),
            new Claim(ClaimTypes.Email, user.Email)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }
}