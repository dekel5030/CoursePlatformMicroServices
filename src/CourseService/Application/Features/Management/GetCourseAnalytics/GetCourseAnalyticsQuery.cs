using Kernel.Messaging.Abstractions;

namespace Courses.Application.Features.Management.GetCourseAnalytics;

public sealed record GetCourseAnalyticsQuery(Guid CourseId) : IQuery<CourseDetailedAnalyticsDto>;
