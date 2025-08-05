using CourseService.Dtos;
using CourseService.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Extentions;

public static class CourseQueryableExtensions
{
    public static IQueryable<Course> ApplySearchFilters(this IQueryable<Course> query, CourseSearchDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.Title))
        {
            query = query.Where(c => EF.Functions.ILike(c.Title, $"%{dto.Title}%"));
        }

        if (!string.IsNullOrWhiteSpace(dto.Description))
        {
            query = query.Where(c => c.Description != null &&
                                     EF.Functions.ILike(c.Description, $"%{dto.Description}%"));
        }

        if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
        {
            query = query.Where(c => c.ImageUrl != null &&
                                     EF.Functions.ILike(c.ImageUrl, $"%{dto.ImageUrl}%"));
        }

        if (dto.InstructorUserId.HasValue)
        {
            query = query.Where(c => c.InstructorUserId == dto.InstructorUserId.Value);
        }

        if (dto.IsPublished.HasValue)
        {
            query = query.Where(c => c.IsPublished == dto.IsPublished.Value);
        }

        if (dto.Price.HasValue)
        {
            query = query.Where(c => c.Price == dto.Price.Value);
        }

        var pageNumber = dto.PageNumber;
        var pageSize = dto.PageSize;

        query = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return query;
    }
}
