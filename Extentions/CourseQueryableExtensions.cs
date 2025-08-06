using CourseService.Dtos.Courses;
using CourseService.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Extentions;

public static class CourseQueryableExtensions
{
    public static IQueryable<Course> ApplySearchFilters(this IQueryable<Course> query, CourseSearchDto dto)
    {
        query = query.ApplyTitleFilter(dto.Title);
        query = query.ApplyDescriptionFilter(dto.Description);
        query = query.ApplyInstructorUserIdFilter(dto.InstructorUserId);
        query = query.ApplyIsPublishedFilter(dto.IsPublished);
        query = query.ApplyPriceFilter(dto.Price);
        query = query.ApplyPagination(dto.PageNumber, dto.PageSize);

        return query;
    }

    private static IQueryable<Course> ApplyTitleFilter(this IQueryable<Course> query, string? title)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(c => EF.Functions.ILike(c.Title, $"%{title}%"));
        }
        return query;
    }

    private static IQueryable<Course> ApplyDescriptionFilter(this IQueryable<Course> query, string? description)
    {
        if (!string.IsNullOrWhiteSpace(description))
        {
            query = query.Where(c => c.Description != null &&
                                     EF.Functions.ILike(c.Description, $"%{description}%"));
        }
        return query;
    }

    private static IQueryable<Course> ApplyInstructorUserIdFilter(this IQueryable<Course> query, int? instructorUserId)
    {
        if (instructorUserId.HasValue)
        {
            query = query.Where(c => c.InstructorUserId == instructorUserId.Value);
        }
        return query;
    }

    private static IQueryable<Course> ApplyIsPublishedFilter(this IQueryable<Course> query, bool? isPublished)
    {
        if (isPublished.HasValue)
        {
            query = query.Where(c => c.IsPublished == isPublished.Value);
        }
        return query;
    }

    private static IQueryable<Course> ApplyPriceFilter(this IQueryable<Course> query, decimal? price)
    {
        if (price.HasValue)
        {
            query = query.Where(c => c.Price == price.Value);
        }
        return query;
    }

    private static IQueryable<Course> ApplyPagination(this IQueryable<Course> query, int pageNumber, int pageSize)
    {
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }
}
