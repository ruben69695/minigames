using MediatR;
using Microsoft.Extensions.Logging;
using Minigames.Application.Dto;
using Minigames.Core;
using Minigames.Core.Repositories;

namespace Minigames.Application.UseCases.Queries;

public record GetUserGameDataRequest(string userId) : IRequest<UserGameDataDto>;

public class GetUserGameDataHandler : IRequestHandler<GetUserGameDataRequest, UserGameDataDto>
{
    private readonly ILogger<GetUserGameDataHandler> _logger;
    private readonly IUserGameDataRepository _gameDataRepository;

    public GetUserGameDataHandler(ILogger<GetUserGameDataHandler> logger, IUserGameDataRepository gameDataRepository)
    {
        _logger = logger;
        _gameDataRepository = gameDataRepository;
    }

    public async Task<UserGameDataDto> Handle(GetUserGameDataRequest request, CancellationToken cancellationToken)
    {
        var userGameData = await _gameDataRepository.GetAsync(request.userId);

        if (userGameData == null)
        {
            userGameData = new UserGameData(request.userId);
           
            _gameDataRepository.Add(userGameData);
            _logger.LogInformation($"Creating game data for user with id {request.userId}");

            await _gameDataRepository.SaveEntitiesAsync(cancellationToken);
        }

        return new UserGameDataDto(request.userId, userGameData.TotalPoints, userGameData.Record);
    }
}