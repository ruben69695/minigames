using Minigames.Application;
using Minigames.Application.Common.Interfaces;
using Minigames.Application.UseCases.Commands;
using Minigames.Core;

namespace Minigames.Tests.Application.Commands;

public class SignUpHandlerTests : TestBase
{
    [Fact]
    public async Task Given_user_signup_correctly_then_return_access_data()
    {
        // Arrange
        var request = new SignUpRequest("email", "username", "password", "password");
        var sut = _mockProvider.Create<SignUpHandler>();

        var fakeUser = new User { Email = request.email, Id = "2", UserName = "username" };
        var fakeIdentityService = _mockProvider.Mock<IIdentityService>();

        fakeIdentityService
            .Setup(s => s.GetUserByEmailAsync(request.email))
            .ReturnsAsync(default(User));

        fakeIdentityService
            .Setup(s => s.CreateUserAsync(request.email, request.username, request.password, UserRoles.User))
            .ReturnsAsync(fakeUser);

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
    public async Task Given_user_signup_and_email_already_exists_then_throw_exception()
    {
        // Arrange
        var request = new SignUpRequest("email", "username", "password", "password");
        var sut = _mockProvider.Create<SignUpHandler>();

        var fakeIdentityService = _mockProvider.Mock<IIdentityService>();

        fakeIdentityService
            .Setup(s => s.GetUserByEmailAsync(request.email))
            .ReturnsAsync(new User());

        // Act
        Func<Task> act = async () => await sut.Handle(request, default);

        // Assert
        await act.Should()
            .ThrowAsync<AppException>()
            .Where(e => e.Code == System.Net.HttpStatusCode.Conflict)
            .WithMessage("There is already a user registered with this email");
    }

    [Fact]
    public async Task Given_user_signup_and_passwords_dont_match_then_throw_exception()
    {
        // Arrange
        var request = new SignUpRequest("email", "username", "password", "pass");
        var sut = _mockProvider.Create<SignUpHandler>();

        var fakeIdentityService = _mockProvider.Mock<IIdentityService>();

        fakeIdentityService
            .Setup(s => s.GetUserByEmailAsync(request.email))
            .ReturnsAsync(default(User));
        
        // Act
        Func<Task> act = async () => await sut.Handle(request, default);

        // Assert
        await act.Should()
            .ThrowAsync<AppException>()
            .Where(e => e.Code == System.Net.HttpStatusCode.BadRequest)
            .WithMessage("Passwords do not match");
    }
}