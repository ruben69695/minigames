using Minigames.Application;
using Minigames.Application.Common.Interfaces;
using Minigames.Application.UseCases.Commands;
using Minigames.Core;

namespace Minigames.Tests.Application.Commands;

public class LoginHandlerTests : TestBase
{
    [Fact]
    public async Task Given_login_correct_then_return_access_data()
    {
        // Arrange
        var request = new LoginRequest("email", "pass");
        var sut = _mockProvider.Create<LoginHandler>();

        var fakeUser = new User { Email = request.email, Id = "2", UserName = "username" };
        var fakeIdentityService = _mockProvider.Mock<IIdentityService>();

        fakeIdentityService
            .Setup(s => s.GetUserByEmailAsync(request.email))
            .ReturnsAsync(fakeUser);

        fakeIdentityService
            .Setup(s => s.CheckUserPasswordAsync(fakeUser, request.password))
            .ReturnsAsync(true);

        fakeIdentityService
            .Setup(s => s.AuthorizeAsync(fakeUser))
            .ReturnsAsync("token");

        
        // Act
        var result = await sut.Handle(request, default);

        // Assert
        result.id.Should().Be(fakeUser.Id);
        result.username.Should().Be(fakeUser.UserName);
        result.token.Should().Be("token");
    }

    [Fact]
    public async Task Given_login_and_email_does_not_exists_then_throw_exception()
    {
        // Arrange
        var request = new LoginRequest("email", "pass");
        var sut = _mockProvider.Create<LoginHandler>();

        var fakeIdentityService = _mockProvider.Mock<IIdentityService>();

        fakeIdentityService
            .Setup(s => s.GetUserByEmailAsync(request.email))
            .ReturnsAsync(default(User));

        // Act
        Func<Task> act = async () => await sut.Handle(request, default);

        // Assert
        await act.Should().ThrowAsync<AppException>()
            .Where(e => e.Code == System.Net.HttpStatusCode.Forbidden)
            .WithMessage("The email or password are incorrect");
    }

    [Fact]
    public async Task Given_login_and_password_is_not_correct_then_throw_exception()
    {
                // Arrange
        var request = new LoginRequest("email", "pass");
        var sut = _mockProvider.Create<LoginHandler>();

        var fakeUser = new User { Email = request.email, Id = "2", UserName = "username" };
        var fakeIdentityService = _mockProvider.Mock<IIdentityService>();

        fakeIdentityService
            .Setup(s => s.GetUserByEmailAsync(request.email))
            .ReturnsAsync(fakeUser);

        fakeIdentityService
            .Setup(s => s.CheckUserPasswordAsync(fakeUser, request.password))
            .ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await sut.Handle(request, default);

        // Assert
        await act.Should().ThrowAsync<AppException>()
            .Where(e => e.Code == System.Net.HttpStatusCode.Forbidden)
            .WithMessage("The email or password are incorrect");
    }
}