using System;
using System.Collections.Generic;
using System.Text;
using Courses.Application.Courses.Dtos;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses.Primitives;
using Kernel;

namespace Courses.Application.Courses.ReadModels;

public class CoursePageReadModel
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required Guid InstructorId { get; init; }
    public required string InstructorName { get; init; }
    public string? InstructorAvatarUrl { get; init; }
    public required CourseStatus Status { get; init; }
    public required decimal PriceAmount { get; init; }
    public required string PriceCurrency { get; init; }
    public required int EnrollmentCount { get; init; }
    public required int LessonsCount { get; init; }
    public required TimeSpan Duration { get; init; }
    public required DateTimeOffset UpdatedAtUtc { get; init; }

    public required List<string> ImageUrls { get; init; }
    public required List<string> Tags { get; init; }

    public required Guid CategoryId { get; init; }
    public required string CategoryName { get; init; }
    public required string CategorySlug { get; init; }

    public required List<ModuleReadModel> Modules { get; init; }

    public CoursePageDto ToDto()
    {
        return new CoursePageDto
        {
            Id = Id,
            Title = Title,
            Description = Description,
            InstructorId = InstructorId,
            InstructorName = InstructorName,
            InstructorAvatarUrl = InstructorAvatarUrl,
            Status = Status,
            Price = new Money(PriceAmount, PriceCurrency),
            EnrollmentCount = EnrollmentCount,
            LessonsCount = LessonsCount,
            TotalDuration = Duration,
            UpdatedAtUtc = UpdatedAtUtc,
            ImageUrls = ImageUrls.AsReadOnly(),
            Tags = Tags.AsReadOnly(),
            CategoryId = CategoryId,
            CategoryName = CategoryName,
            CategorySlug = CategorySlug,
            Modules = Modules.ConvertAll(m => m.ToDto()),
            Links = []
        };
    }
}
