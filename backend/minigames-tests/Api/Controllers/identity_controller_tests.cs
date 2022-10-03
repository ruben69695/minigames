using MediatR;
using Minigames.Api.Controllers;
using Minigames.Application.UseCases.Queries;
using Minigames.Application.Dto;
using Microsoft.AspNetCore.Mvc;
using Minigames.Application.UseCases.Commands;

namespace Minigames.Tests.Api.Controllers;

public class IdentityControllerTests : TestBase
{
    [Fact]
    public async Task Given_get_user_information_then_return_ok()
    {
        // Arrange
        var sut = _mockProvider.Create<IdentityController>();
        _mockProvider.Mock<IMediator>()
            .Setup(x => x.Send(It.IsAny<GetUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDto("id", "email", true));

        // Act
        var result = await sut.GetUser();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Given_user_logins_then_return_ok()
    {
        // Arrange
        var sut = _mockProvider.Create<IdentityController>();
        var request = new LoginRequest("email", "password");
        _mockProvider.Mock<IMediator>()
            .Setup(x => x.Send(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccessDto("id", "username", "token"));

        // Act
        var result = await sut.Login(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Given_user_signs_up_then_return_ok()
    {
        // Arrange
        var sut = _mockProvider.Create<IdentityController>();
        var request = new SignUpRequest("email", "username", "pass", "pass");
        _mockProvider.Mock<IMediator>()
            .Setup(x => x.Send(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccessDto("id", "username", "token"));

        // Act
        var result = await sut.SignUp(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
}