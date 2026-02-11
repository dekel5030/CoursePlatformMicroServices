using Courses.Application.Courses.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetCourseAnalytics;

public sealed record GetCourseAnalyticsQuery(Guid CourseId) : IQuery<CourseDetailedAnalyticsDto>;
