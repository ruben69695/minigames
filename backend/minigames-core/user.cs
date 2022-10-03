using Microsoft.AspNetCore.Identity;

namespace Minigames.Core;

public class User : IdentityUser
{
    public static User Create(string email, string username)
    {
        return new User { Email = email, UserName = username };
    }
}