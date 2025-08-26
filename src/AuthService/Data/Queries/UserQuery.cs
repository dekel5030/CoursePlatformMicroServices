namespace AuthService.Data.Queries;

using AuthService.Models;

public class UserQuery
{
    public string? Email { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsConfirmed { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; } 

    public IQueryable<AuthUser> Apply(IQueryable<AuthUser> query)
    {
        query = ApplyEmailFilter(query);
        query = ApplyIsActiveFilter(query);
        query = ApplyIsConfirmedFilter(query);

        return query;
    }

    private IQueryable<AuthUser> ApplyEmailFilter(IQueryable<AuthUser> query)
    {
        if (!string.IsNullOrEmpty(Email))
        {
            query = query.Where(u => u.Email.Contains(Email));
        }
        return query;
    }

    private IQueryable<AuthUser> ApplyIsActiveFilter(IQueryable<AuthUser> query)
    {
        if (IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == IsActive.Value);
        }
        return query;
    }

    private IQueryable<AuthUser> ApplyIsConfirmedFilter(IQueryable<AuthUser> query)
    {
        if (IsConfirmed.HasValue)
        {
            query = query.Where(u => u.IsConfirmed == IsConfirmed.Value);
        }
        return query;
    }
}
