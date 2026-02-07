using Kernel.Messaging.Abstractions;

namespace Users.Application.LecturerProfiles.Commands.CreateLecturerProfile;

public sealed record CreateLecturerProfileCommand(
    Guid UserId,
    string? ProfessionalBio,
    string? Expertise,
    int YearsOfExperience) : ICommand<LecturerProfileResponseDto>
{ }

public sealed record LecturerProfileResponseDto(
    Guid Id,
    Guid UserId,
    string? ProfessionalBio,
    string? Expertise,
    int YearsOfExperience);
