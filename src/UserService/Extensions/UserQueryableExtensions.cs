using UserService.Dtos;
using UserService.Models;

namespace UserService.Extensions
{
    public static class UserQueryableExtensions
    {
        public static IQueryable<User> ApplySearchFilters(this IQueryable<User> query, UserSearchDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Email))
                query = query.Where(u => u.Email.Contains(dto.Email));

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                query = query.Where(u => u.FullName.Contains(dto.FullName));

            if (dto.CreatedAtFrom.HasValue)
                query = query.Where(u => u.CreatedAt >= dto.CreatedAtFrom);

            if (dto.CreatedAtTo.HasValue)
                query = query.Where(u => u.CreatedAt <= dto.CreatedAtTo);

            if (dto.UpdatedAtFrom.HasValue)
                query = query.Where(u => u.UpdatedAt >= dto.UpdatedAtFrom);

            if (dto.UpdatedAtTo.HasValue)
                query = query.Where(u => u.UpdatedAt <= dto.UpdatedAtTo);

            return query;
        }
    }
}