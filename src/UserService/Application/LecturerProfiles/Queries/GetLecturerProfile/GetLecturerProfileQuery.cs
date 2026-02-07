using Kernel.Messaging.Abstractions;
using Users.Application.LecturerProfiles.Queries.Dtos;

namespace Users.Application.LecturerProfiles.Queries.GetLecturerProfile;

public sealed record GetLecturerProfileQuery(Guid UserId) : IQuery<LecturerProfileDto>;
