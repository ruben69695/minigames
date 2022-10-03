using System.Security.Claims;
namespace Minigames.Application.Common.Interfaces;

public interface IAuthorizationService
{
    string CreateAuthorization(IEnumerable<Claim> claims);
}