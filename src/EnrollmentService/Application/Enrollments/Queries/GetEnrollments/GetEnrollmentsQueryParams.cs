namespace Application.Enrollments.Queries.GetEnrollments;

public record GetEnrollmentsQueryParams(
    string? UserId,
    string? CourseId,
    string? Status,
    int PageNumber = 1,
    int PageSize = 10);
