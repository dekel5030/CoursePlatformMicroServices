using AutoMapper;
using FluentAssertions;
using Moq;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;
using UserService.Services;
using Xunit;
using Common.Errors;

namespace UserService.Tests
{
    public class DeleteUserAsyncTests
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserService.Services.UserService _userService;

        public DeleteUserAsyncTests()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService.Services.UserService(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task ShouldReturnUserNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 1;
            _repositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null!);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(Error.UserNotFound);
        }

        [Fact]
        public async Task ShouldDeleteAndReturnUser_WhenUserExists()
        {
            // Arrange
            int userId = 1;
            var user = new User
            {
                Email = "test@example.com",
                FullName = "Test User",
                PasswordHash = "hashed"
            };
            var userReadDto = new UserReadDto
            {
                Id = 1, // userId will be mapped by AutoMapper from the actual user
                Email = user.Email,
                FullName = user.FullName
            };

            _repositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _repositoryMock.Setup(r => r.DeleteUserAsync(userId)).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
            _mapperMock.Setup(m => m.Map<UserReadDto>(user)).Returns(userReadDto);

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(userReadDto);

            _repositoryMock.Verify(r => r.DeleteUserAsync(userId), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
