using Minigames.Core;

namespace Minigames.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> CreateUserAsync(string email, string username, string password, params string[] roles);
    Task<IEnumerable<string>> GetUserRolesAsync(User user);
    Task<bool> CheckUserPasswordAsync(User user, string password);
    Task<string> AuthorizeAsync(User user);
}