using System.Security.Claims;
using Minigames.Core;

namespace Minigames.Application.Common.Interfaces;

public interface IUserClaimsFactory<TUser> where TUser : class
{
    IEnumerable<Claim> Create(TUser user, IEnumerable<string> roles);
}