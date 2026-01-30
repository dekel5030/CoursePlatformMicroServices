using System.Globalization;
using CoursePlatform.Contracts.UserEvents;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Kernel.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Courses.Application.EventConsumers;

/// <summary>
/// Projector for InstructorReadModel - synchronizes user data from UserService.
/// Uses ONLY Integration Events from UserService (no WriteDbContext dependency).
/// </summary>
internal sealed class InstructorProjector : IEventConsumer<UserCreated>
{
    private readonly IReadDbContext _readDbContext;
    private readonly ILogger<InstructorProjector> _logger;

    public InstructorProjector(
        IReadDbContext readDbContext,
        ILogger<InstructorProjector> logger)
    {
        _readDbContext = readDbContext;
        _logger = logger;
    }

    public async Task HandleAsync(UserCreated message, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(message.UserId, out Guid userId))
        {
            _logger.LogWarning("Received UserCreated event with invalid UserId: {UserId}", message.UserId);
            return;
        }

        InstructorReadModel? existing = await _readDbContext.Instructors
            .FirstOrDefaultAsync(i => i.Id == userId, cancellationToken);

        if (existing is not null)
        {
            // Upsert behavior - update if exists
            _logger.LogInformation("Instructor with ID {UserId} already exists. Updating", userId);

            existing.FirstName = message.FirstName;
            existing.LastName = message.LastName;
            existing.Email = message.Email;
            existing.FullName = FormatFullName(message.FirstName, message.LastName);
            existing.AvatarUrl = message.AvatarUrl;
            existing.UpdatedAtUtc = DateTimeOffset.UtcNow;

            await _readDbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var instructor = new InstructorReadModel
        {
            Id = userId,
            FirstName = message.FirstName,
            LastName = message.LastName,
            Email = message.Email,
            FullName = FormatFullName(message.FirstName, message.LastName),
            AvatarUrl = message.AvatarUrl,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        _readDbContext.Instructors.Add(instructor);
        await _readDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Instructor with ID {UserId} created successfully", userId);
    }

    private static string FormatFullName(string firstName, string lastName)
    {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        string first = textInfo.ToTitleCase(firstName.ToLower());
        string last = textInfo.ToTitleCase(lastName.ToLower());
        return $"{first} {last}";
    }
}
