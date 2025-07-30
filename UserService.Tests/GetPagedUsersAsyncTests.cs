using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using UserService.Common.Errors;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;
using UserService.Services;
using Xunit;

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
        public async Task Should_ReturnFailure_WhenPageNumberOrPageSizeIsInvalid()
        {
            // Arrange
            int pageNumber = 0;
            int pageSize = -5;

            // Act
            var result = await _userService.GetPagedUsersAsync(pageNumber, pageSize);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCode.InvalidPageNumberOrSize);
        }

        [Fact]
        public async Task Should_ReturnPagedUsers_WhenValidInput()
        {
            // Arrange
            int pageNumber = 2;
            int pageSize = 3;
            int skip = (pageNumber - 1) * pageSize;

            var fakeUsers = new List<User>
            {
                new User { FullName = "User One", Email = "test1@gmail.com", PasswordHash = "123"},
                new User { FullName = "User Two", Email = "test2@gmail.com", PasswordHash = "123"},
                new User { FullName = "User Three", Email = "test3@gmail.com", PasswordHash = "123"}
            };

            var fakeUserDtos = fakeUsers.Select(u => new UserReadDto { Id = u.Id, FullName = u.FullName, Email = u.Email });

            _repositoryMock
                .Setup(r => r.GetPagedUsersAsync(skip, pageSize))
                .ReturnsAsync(fakeUsers);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<UserReadDto>>(fakeUsers))
                .Returns(fakeUserDtos);

            // Act
            var result = await _userService.GetPagedUsersAsync(pageNumber, pageSize);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(3);
            result.Value.Select(u => u.FullName).Should().Contain(new[] { "User One", "User Two", "User Three" });
        }
    }
}
