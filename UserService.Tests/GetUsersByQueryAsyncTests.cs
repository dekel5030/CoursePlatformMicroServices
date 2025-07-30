using AutoMapper;
using FluentAssertions;
using Moq;
using UserService.Common;
using UserService.Common.Errors;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;
using Xunit;

namespace UserService.Tests
{
    public class GetUsersByQueryAsyncTests
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Services.UserService _userService;

        public GetUsersByQueryAsyncTests()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _userService = new Services.UserService(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task ShouldReturnFailure_WhenPageNumberIsZero()
        {
            // Arrange
            var query = new UserSearchDto { PageNumber = 0, PageSize = 10 };

            // Act
            var result = await _userService.GetUsersByQueryAsync(query);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCode.InvalidPageNumberOrSize);
        }

        [Fact]
        public async Task ShouldReturnFailure_WhenPageSizeIsNegative()
        {
            var query = new UserSearchDto { PageNumber = 1, PageSize = -5 };

            var result = await _userService.GetUsersByQueryAsync(query);

            result.IsSuccess.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCode.InvalidPageNumberOrSize);
        }

        [Fact]
        public async Task ShouldReturnMappedUsers_WhenQueryIsValid()
        {
            // Arrange
            var query = new UserSearchDto { PageNumber = 1, PageSize = 10 };
            var users = new List<User>
            {
                new User { FullName = "Test User", Email = "test@gmail.com", PasswordHash = "123" }
            };
            var userDtos = new List<UserDetailsDto>
            {
                new UserDetailsDto { Id = 1, FullName = "Test User", Email = "test@gmail.com" }
            };

            _repositoryMock.Setup(r => r.SearchUsersAsync(query)).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserDetailsDto>>(users)).Returns(userDtos);

            // Act
            var result = await _userService.GetUsersByQueryAsync(query);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(userDtos);
        }

        [Fact]
        public async Task ShouldCallRepositoryAndMapperOnce_WhenQueryIsValid()
        {
            var query = new UserSearchDto { PageNumber = 1, PageSize = 5 };
            var users = new List<User>();
            var userDtos = new List<UserDetailsDto>();

            _repositoryMock.Setup(r => r.SearchUsersAsync(query)).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserDetailsDto>>(users)).Returns(userDtos);

            var result = await _userService.GetUsersByQueryAsync(query);

            _repositoryMock.Verify(r => r.SearchUsersAsync(query), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<UserDetailsDto>>(users), Times.Once);
        }
    }
}
