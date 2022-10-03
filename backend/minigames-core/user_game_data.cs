using System.ComponentModel.DataAnnotations;

namespace Minigames.Core;

public class UserGameData
{
    public string UserId { get; set; }
    public User? User { get; set; }

    public int TotalPoints { get; set; }
    public int Record { get; set; }
    public DateTime? LastGameDate { get; set; }

    public UserGameData(string userId)
    {
        UserId = userId;
    }
}