using Minigames.Core;
using Minigames.Core.Repositories;

namespace Minigames.Infrastructure.Repositories;

public class UserGameDataRepository : RepositoryBase<UserGameData, string>, IUserGameDataRepository
{
    private readonly MinigamesContext _dbContext;

    public UserGameDataRepository(MinigamesContext context) : base(context)
    {
        _dbContext = context;
    }
}