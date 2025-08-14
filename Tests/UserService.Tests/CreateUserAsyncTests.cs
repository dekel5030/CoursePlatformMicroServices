using AutoMapper;
using FluentAssertions;
using Moq;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;
using Common.Errors;
using Xunit;

namespace UserService.Tests
{
    public class CreateUserAsyncTests
    {
        readonly Mock<IUserRepository> _repositoryMock;
        readonly Mock<IMapper> _mapperMock;
        readonly UserService.Services.UserService _userService;
        public CreateUserAsyncTests()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService.Services.UserService(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task ShouldReturnDuplicateEmail_WhenEmailAlreadyExists()
        {
            var userCreateDto = new UserCreateDto { FullName = "test", Email = "test@gmai.com", PasswordHash = "hashedpassword" };

            _repositoryMock.Setup(r => r.EmailExistsAsync(userCreateDto.Email)).ReturnsAsync(true);

            var result = await _userService.CreateUserAsync(userCreateDto);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(Error.DuplicateEmail);
            _repositoryMock.Verify(r => r.EmailExistsAsync(userCreateDto.Email), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ShouldReturnSuccess_WhenEmailIsUnique()
        {
            var userCreateDto = new UserCreateDto { FullName = "test", Email = "test@gmail.com", PasswordHash = "hashedpassword" };
            var user = new User { FullName = "test", Email = "test@gmail.com", PasswordHash = "hashedpassword" };
            var userReadDto = new UserReadDto { FullName = "test", Email = "test@gmail.com", Id = 1 };

            _repositoryMock.Setup(r => r.EmailExistsAsync(userCreateDto.Email)).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<User>(userCreateDto)).Returns(user);
            _mapperMock.Setup(m => m.Map<UserReadDto>(user)).Returns(userReadDto);
            var result = await _userService.CreateUserAsync(userCreateDto);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(userReadDto);
            _repositoryMock.Verify(r => r.EmailExistsAsync(userCreateDto.Email), Times.Once);
            _repositoryMock.Verify(r => r.AddUserAsync(user), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}