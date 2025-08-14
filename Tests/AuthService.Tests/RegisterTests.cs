using AutoMapper;
using AuthService.Data;
using AuthService.Dtos;
using AuthService.Models;
using AuthService.Security;
using AuthService.SyncDataServices.Grpc;
using Common;
using Common.Errors;
using Common.Rollback;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AuthService.Profiles;

namespace AuthService.Tests;

public class RegisterTests
{
    private readonly Mock<IAuthRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IRollbackManager> _rollbackManagerMock = new();
    private readonly Mock<ILogger<AuthService.Services.AuthService>> _loggerMock = new();
    private readonly Mock<IGrpcUserServiceDataClient> _usersClientMock = new();

    private readonly AuthService.Services.AuthService _authService;

    public RegisterTests()
    {
        _authService = new AuthService.Services.AuthService(
            _repositoryMock.Object,
            _mapperMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object,
            _rollbackManagerMock.Object,
            _loggerMock.Object,
            _usersClientMock.Object
        );
    }

    [Fact]
    public async Task ShouldReturnFailure_WhenEmailAlreadyExists()
    {
        // Arrange
        var email = "test@example.com";
        _repositoryMock.Setup(r => r.GetUserCredentialsByEmailAsync(email))
            .ReturnsAsync(new UserCredentials() { UserId = 1, Email = email, PasswordHash = "hashedPassword" });

        var dto = new RegisterRequestDto { Email = email, Password = "1234", FullName = "Test", PasswordConfirm = "1234" };

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(Error.DuplicateEmail);
    }

    [Fact]
    public async Task ShouldReturnFailure_WhenUserServiceFails()
    {
        // Given
        var RegisterRequestDto = new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "1234",
            FullName = "Test",
            PasswordConfirm = "1234"
        };
        var userCreateDto = new UserCreateDto
        {
            Email = RegisterRequestDto.Email,
            FullName = RegisterRequestDto.FullName,
            PasswordHash = RegisterRequestDto.Password
        };

        // When
        _usersClientMock
            .Setup(c => c.CreateUserAsync(userCreateDto))
            .ReturnsAsync(Result<UserReadDto>.Failure(Error.InvalidPageSize));
        _mapperMock
            .Setup(m => m.Map<UserCreateDto>(RegisterRequestDto))
            .Returns(userCreateDto);

        var result = await _authService.RegisterAsync(RegisterRequestDto);

        // Then
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(Error.InvalidPageSize);
    }

    [Fact]
    public async Task ShouldRollBack_WhenAuthDbFails()
    {
        // Arrange
        var RegisterRequestDto = new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "1234",
            FullName = "Test",
            PasswordConfirm = "1234"
        };
        var userCreateDto = new UserCreateDto
        {
            Email = RegisterRequestDto.Email,
            FullName = RegisterRequestDto.FullName,
            PasswordHash = RegisterRequestDto.Password
        };
        var userReadDto = new UserReadDto
        {
            Id = 1,
            Email = RegisterRequestDto.Email,
            FullName = RegisterRequestDto.FullName
        };
        var UserCredentials = new UserCredentials
        {
            UserId = 1,
            Email = RegisterRequestDto.Email,
            PasswordHash = RegisterRequestDto.Password
        };

        _mapperMock
            .Setup(m => m.Map<UserCreateDto>(RegisterRequestDto))
            .Returns(userCreateDto);

        _mapperMock
            .Setup(m => m.Map<UserCredentials>(userReadDto))
            .Returns(UserCredentials);

        _usersClientMock
            .Setup(c => c.CreateUserAsync(userCreateDto))
            .ReturnsAsync(Result<UserReadDto>.Success(userReadDto));

        _repositoryMock
            .Setup(m => m.SaveChangesAsync())
            .ThrowsAsync(new Exception("Database error"));

        var result = await _authService.RegisterAsync(RegisterRequestDto);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(Error.DatabaseError);
        _rollbackManagerMock.Verify(m => m.ExecuteAllAsync(true), Times.Once);
    }

    [Fact]
    public async Task ShouldCreateUser_WhenRequestIsValid()
    {
        // Arrange
        var RegisterRequestDto = new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "1234",
            FullName = "Test",
            PasswordConfirm = "1234"
        };
        var userCreateDto = new UserCreateDto
        {
            Email = RegisterRequestDto.Email,
            FullName = RegisterRequestDto.FullName,
            PasswordHash = RegisterRequestDto.Password
        };
        var userReadDto = new UserReadDto
        {
            Id = 1,
            Email = RegisterRequestDto.Email,
            FullName = RegisterRequestDto.FullName
        };
        var UserCredentials = new UserCredentials
        {
            UserId = 1,
            Email = RegisterRequestDto.Email,
            PasswordHash = RegisterRequestDto.Password
        };
        var authResponse = new AuthResponseDto
        {
            Email = userReadDto.Email,
            Token = "some-jwt-token",
            UserId = userReadDto.Id,
        };

        _mapperMock
            .Setup(m => m.Map<UserCreateDto>(RegisterRequestDto))
            .Returns(userCreateDto);

        _mapperMock
            .Setup(m => m.Map<UserCredentials>(userReadDto))
            .Returns(UserCredentials);

        _mapperMock
            .Setup(m => m.Map<AuthResponseDto>(UserCredentials))
            .Returns(authResponse);

        _usersClientMock
            .Setup(c => c.CreateUserAsync(userCreateDto))
            .ReturnsAsync(Result<UserReadDto>.Success(userReadDto));


        var result = await _authService.RegisterAsync(RegisterRequestDto);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(authResponse);
        _rollbackManagerMock.Verify(m => m.ExecuteAllAsync(true), Times.Never);
    }
}
