using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;
using Xunit;
using Common.Errors;

namespace UserService.Tests
{
    public class SetUserActivationAsyncTests
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserService.Services.UserService _userService;

        public SetUserActivationAsyncTests()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService.Services.UserService(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task ShouldReturnResultFalied_WhenUserNotFound()
        {
            // Given
            int userId = 1;
            _repositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

            // When
            var result = await _userService.SetUserActivationAsync(userId, true);

            // Then
            result.Error.Should().Be(Error.UserNotFound);
            result.IsSuccess.Should().BeFalse();
            _repositoryMock.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
        
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ShouldSetUserActivationCorrectly_WhenUserExists(bool IsActive)
        {
            int userId = 1;
            var User = new User { Email = "Test@gmail.com", FullName = "Test User", PasswordHash = "hashedpassword" };
            var UserReadDto = new UserReadDto { Email = User.Email, FullName = User.FullName, Id = userId };

            _repositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(User);
            _mapperMock.Setup(m => m.Map<UserReadDto>(User)).Returns(UserReadDto);
            var result = await _userService.SetUserActivationAsync(userId, IsActive);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(UserReadDto);
            User.IsActive.Should().Be(IsActive);
            _repositoryMock.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<UserReadDto>(User), Times.Once);
        }
    }
}