using Minigames.Application;
using Minigames.Application.UseCases.Commands;
using Minigames.Core;
using Minigames.Core.Repositories;

namespace Minigames.Tests.Application.Commands;

public class UpdateUserGameDataHandlerTests : TestBase
{
    [Fact]
    public async Task Given_user_update_game_data_then_return_data_updated()
    {
        // Arrange
        var sut = _mockProvider.Create<UpdateUserGameDataHandler>();
        var request = new UpdateUserGameDataRequest { Points = 200 };
        request.SetUserId("1");
        var fakeData = new UserGameData(request.UserId!) { TotalPoints = 3000, Record = 360, LastGameDate = null };

        var repo = _mockProvider.Mock<IUserGameDataRepository>();
        repo.Setup(r => r.GetAsync(request.UserId!))
            .ReturnsAsync(fakeData);

        // Act
        var result = await sut.Handle(request, default);

        // Assert
        result.userId.Should().Be("1");
        result.totalPoints.Should().Be(3200);
        result.record.Should().Be(360);
        fakeData.LastGameDate.Should().NotBeNull();

        repo.Verify(r => r.Update(fakeData), Times.Once);
        repo.Verify(r => r.SaveEntitiesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Given_user_update_game_data_and_get_new_record_then_return_data_updated()
    {
        // Arrange
        var sut = _mockProvider.Create<UpdateUserGameDataHandler>();
        var request = new UpdateUserGameDataRequest { Points = 361 };
        var date = DateTime.UtcNow.AddDays(-1);
        request.SetUserId("1");
        var fakeData = new UserGameData(request.UserId!) { TotalPoints = 3000, Record = 360, LastGameDate = date };

        var repo = _mockProvider.Mock<IUserGameDataRepository>();
        repo.Setup(r => r.GetAsync(request.UserId!))
            .ReturnsAsync(fakeData);

        // Act
        var result = await sut.Handle(request, default);

        // Assert
        result.userId.Should().Be("1");
        result.totalPoints.Should().Be(3361);
        result.record.Should().Be(361);
        fakeData.LastGameDate.Should().NotBeSameDateAs(date);

        repo.Verify(r => r.Update(fakeData), Times.Once);
        repo.Verify(r => r.SaveEntitiesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Given_user_update_game_data_and_there_is_no_game_data_then_throw_exception()
    {
        // Arrange
        var sut = _mockProvider.Create<UpdateUserGameDataHandler>();
        var request = new UpdateUserGameDataRequest { Points = 361 };
        request.SetUserId("1");

        var repo = _mockProvider.Mock<IUserGameDataRepository>();
        repo.Setup(r => r.GetAsync(request.UserId!))
            .ReturnsAsync(default(UserGameData));

        // Act
        Func<Task> act = async () => await sut.Handle(request, default);

        // Assert
        await act.Should()
            .ThrowAsync<AppException>()
            .Where(e => e.Code == System.Net.HttpStatusCode.NotFound)
            .WithMessage("User game data not found");

        repo.Verify(r => r.Update(It.IsAny<UserGameData>()), Times.Never);
        repo.Verify(r => r.SaveEntitiesAsync(default), Times.Never);
    }
}