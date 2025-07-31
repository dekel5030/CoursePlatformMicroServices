using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class GetPagedUsersAsyncTests
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserService.Services.UserService _userService;

        public GetPagedUsersAsyncTests()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService.Services.UserService(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Should_ReturnEmptyList_WhenNoUsersReturned()
        {
            // Arrange
            var query = new UserSearchDto { PageNumber = 1, PageSize = 10 };
            var users = new List<User>();
            var userDtos = new List<UserDetailsDto>();

            _repositoryMock.Setup(r => r.SearchUsersAsync(query)).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserDetailsDto>>(users)).Returns(userDtos);

            // Act
            var result = await _userService.GetUsersByQueryAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Should_ReturnUsers_WhenValidInput()
        {
            // Arrange
            var query = new UserSearchDto { PageNumber = 2, PageSize = 3 };
            var fakeUsers = new List<User>
            {
                new User { FullName = "User One", Email = "test1@gmail.com", PasswordHash = "123"},
                new User { FullName = "User Two", Email = "test2@gmail.com", PasswordHash = "123"},
                new User { FullName = "User Three", Email = "test3@gmail.com", PasswordHash = "123"}
            };

            var fakeUserDtos = fakeUsers.Select(u => new UserDetailsDto { Id = u.Id, FullName = u.FullName, Email = u.Email });

            _repositoryMock
                .Setup(r => r.SearchUsersAsync(query))
                .ReturnsAsync(fakeUsers);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<UserDetailsDto>>(fakeUsers))
                .Returns(fakeUserDtos);

            // Act
            var result = await _userService.GetUsersByQueryAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Select(u => u.FullName).Should().Contain(new[] { "User One", "User Two", "User Three" });
        }
    }
}
