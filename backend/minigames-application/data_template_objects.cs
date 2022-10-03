namespace Minigames.Application.Dto;

public record AccessDto(string id, string username, string token);
public record UserDto(string userId, string email, bool isAuthenticated);
public record ClaimDto(string key, string value);
public record UserGameDataDto(string userId, int totalPoints, int record);