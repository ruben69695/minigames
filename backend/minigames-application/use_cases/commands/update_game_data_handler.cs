using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using Minigames.Application.Dto;
using Minigames.Core.Repositories;

namespace Minigames.Application.UseCases.Commands;

public class UpdateUserGameDataRequest : IRequest<UserGameDataDto>
{
    public string? UserId { get; private set; }
    public int Points { get; set; }

    public void SetUserId(string userId)
    {
        UserId = userId;
    }
}

public class UpdateUserGameDataHandler : IRequestHandler<UpdateUserGameDataRequest, UserGameDataDto>
{
    private readonly IUserGameDataRepository _gameDataRepository;
    private readonly ILogger<UpdateUserGameDataHandler> _logger;
    
    public UpdateUserGameDataHandler(IUserGameDataRepository gameDataRepository, ILogger<UpdateUserGameDataHandler> logger)
    {
        _gameDataRepository = gameDataRepository;
        _logger = logger;
    }

    public async Task<UserGameDataDto> Handle(UpdateUserGameDataRequest request, CancellationToken cancellationToken)
    {
        var userGameData = await _gameDataRepository.GetAsync(request.UserId!);

        if (userGameData == null)
        {
            _logger.LogInformation($"User game data not found for user id {request.UserId!}");
            throw new AppException("User game data not found", HttpStatusCode.NotFound);
        }

        userGameData.TotalPoints += request.Points;
        userGameData.LastGameDate = DateTime.UtcNow;

        if (request.Points > userGameData.Record)
        {
            userGameData.Record = request.Points;
        }

        _gameDataRepository.Update(userGameData);
        await _gameDataRepository.SaveEntitiesAsync(cancellationToken);

        return new UserGameDataDto(userGameData.UserId, userGameData.TotalPoints, userGameData.Record);
    }
}

