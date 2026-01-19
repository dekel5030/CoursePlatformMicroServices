using CoursePlatform.Contracts.UserEvents;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Users;
using Kernel.EventBus;
using Microsoft.Extensions.Logging;

namespace Courses.Application.EventConsumers;

internal sealed class UserCreatedEventConsumer : IEventConsumer<UserCreated>
{
    private readonly IUsersRepository _userRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserCreatedEventConsumer> _logger;

    public UserCreatedEventConsumer(
        IUsersRepository userRepo,
        IUnitOfWork unitOfWork,
        ILogger<UserCreatedEventConsumer> logger)
    {
        _userRepo = userRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(UserCreated message, CancellationToken cancellationToken = default)
    {
        if (!UserId.TryParse(message.UserId, out UserId? userId))
        {
            _logger.LogWarning("Received UserCreated event with invalid UserId: {UserId}", message.UserId);
            return;
        }

        User? user = await _userRepo.GetByIdAsync(userId, cancellationToken);

        if (user is not null)
        {
            _logger.LogInformation("User with ID {UserId} already exists. Skipping creation.", userId);
            return;
        }

        _logger.LogInformation("Creating new user with ID {UserId}", userId);

        user = new User
        {
            Id = userId,
            Email = message.Email,
            FirstName = message.FirstName,
            LastName = message.LastName,
            AvatarUrl = message.AvatarUrl
        };

        await _userRepo.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User with ID {UserId} created successfully", userId);
    }
}
