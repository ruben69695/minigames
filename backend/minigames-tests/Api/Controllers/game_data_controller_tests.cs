using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minigames.Api.Controllers;
using Minigames.Application;
using Minigames.Application.UseCases.Commands;

namespace Minigames.Tests.Api.Controllers;

public class GameDataControllerTests : TestBase
{
    [Fact]
    public async Task Given_user_gets_his_game_data_then_return_ok()
    {
        // Arrange
        var sut = _mockProvider.Create<GameDataController>();
        var claims = new[] { new Claim(ClaimTypes.Sid, "2") };
        var fakeUser = new Mock<ClaimsPrincipal>();

        fakeUser.SetupGet(p => p.Claims).Returns(claims);

        _mockProvider.Mock<IHttpContextAccessor>()
            .Setup(p => p.HttpContext!.User)
            .Returns(fakeUser.Object);

        // Act
        var result = await sut.GetUserGameData();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Given_user_gets_his_game_data_and_user_not_exists_then_throw_exception()
    {
        // Arrange
        var sut = _mockProvider.Create<GameDataController>();
        ClaimsPrincipal? claim = null;

        _mockProvider.Mock<IHttpContextAccessor>()
            .Setup(p => p.HttpContext!.User)
            .Returns(claim!);

        // Act
        Func<Task> act = async () => await sut.GetUserGameData();

        // Assert
        await act.Should().ThrowAsync<AppException>().WithMessage("User not found");
    }

    [Fact]
    public async Task Given_user_updates_his_game_data_points_then_return_ok()
    {
        // Arrange
        var sut = _mockProvider.Create<GameDataController>();
        var claims = new[] { new Claim(ClaimTypes.Sid, "2") };
        var fakeUser = new Mock<ClaimsPrincipal>();

        fakeUser.SetupGet(p => p.Claims).Returns(claims);

        _mockProvider.Mock<IHttpContextAccessor>()
            .Setup(p => p.HttpContext!.User)
            .Returns(fakeUser.Object);

        // Act
        var request = new UpdateUserGameDataRequest { Points = 3000 };
        var result = await sut.UpdateUserGameData(request);

        // Assert
        request.UserId.Should().Be("2");
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Given_user_updates_his_game_data_points_and_user_not_exists_then_throw_exception()
    {
        // Arrange
        var sut = _mockProvider.Create<GameDataController>();
        ClaimsPrincipal? claim = null;

        _mockProvider.Mock<IHttpContextAccessor>()
            .Setup(p => p.HttpContext!.User)
            .Returns(claim!);

        // Act
        Func<Task> act = async () => await sut.UpdateUserGameData(new UpdateUserGameDataRequest());

        // Assert
        await act.Should().ThrowAsync<AppException>().WithMessage("User not found");
    }
}