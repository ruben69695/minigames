using Minigames.Application.UseCases.Queries;
using Minigames.Core;
using Minigames.Core.Repositories;

namespace Minigames.Tests.Application.Queries;

public class GetUserGameDataHandlerTests : TestBase
{
    [Fact]
    public async Task Given_get_user_game_data_and_not_exist_then_create_and_return_data()
    {
        // Arrange
        var request = new GetUserGameDataRequest("1");
        var sut = _mockProvider.Create<GetUserGameDataHandler>();
        var repo = _mockProvider.Mock<IUserGameDataRepository>();

        repo
            .Setup(s => s.GetAsync(request.userId))
            .ReturnsAsync(default(UserGameData));

        // Act
        var result = await sut.Handle(request, default);

        // Assert
        result.userId.Should().Be("1");
        result.totalPoints.Should().Be(0);
        result.record.Should().Be(0);

        repo.Verify(v => v.Add(It.IsAny<UserGameData>()), Times.Once);
        repo.Verify(v => v.SaveEntitiesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Given_get_user_game_data_and_exist_then_return_data()
    {
        // Arrange
        var request = new GetUserGameDataRequest("1");
        var sut = _mockProvider.Create<GetUserGameDataHandler>();
        var repo = _mockProvider.Mock<IUserGameDataRepository>();
        var userGameData = new UserGameData("1") { Record = 300, TotalPoints = 4000 };

        repo
            .Setup(s => s.GetAsync(request.userId))
            .ReturnsAsync(userGameData);

        // Act
        var result = await sut.Handle(request, default);

        // Assert
        result.userId.Should().Be("1");
        result.totalPoints.Should().Be(4000);
        result.record.Should().Be(300);

        repo.Verify(v => v.Add(It.IsAny<UserGameData>()), Times.Never);
        repo.Verify(v => v.SaveEntitiesAsync(default), Times.Never);
    }

}