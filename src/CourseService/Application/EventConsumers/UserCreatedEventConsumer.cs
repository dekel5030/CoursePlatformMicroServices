using CoursePlatform.Contracts.UserEvents;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Users;
using Kernel.EventBus;

namespace Courses.Application.EventConsumers;

internal sealed class UserCreatedEventConsumer : IEventConsumer<UserCreated>
{
    private readonly IUsersRepository _userRepo;
    private readonly IUnitOfWork _unitOfWork;

    public UserCreatedEventConsumer(IUsersRepository userRepo, IUnitOfWork unitOfWork)
    {
        _userRepo = userRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(UserCreated message, CancellationToken cancellationToken = default)
    {
        var userId = new UserId(message.UserId);
        User? user = await _userRepo.GetByIdAsync(userId, cancellationToken);

        if (user is not null)
        {
            return;
        }
        string[] splitName = message.Fullname.Split(' ');
        string firstName = splitName[0];
        string lastName = splitName.Length > 1 ? splitName[1] : string.Empty;

        user = new User
        {
            Id = userId,
            Email = message.Email,
            FirstName = firstName,
            LastName = lastName,
        };

        await _userRepo.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
