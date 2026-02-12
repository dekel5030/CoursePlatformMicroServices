using Courses.Application.Courses.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Features.Management.CourseAnalytics;

public sealed record GetCourseAnalyticsQuery(Guid CourseId) : IQuery<CourseDetailedAnalyticsDto>;
