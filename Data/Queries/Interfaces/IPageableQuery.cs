namespace EnrollmentService.Data.Queries.Interfaces;

public interface IPageableQuery<T>
{
    int PageNumber { get; }
    int PageSize { get; }

    IQueryable<T> ApplyPagination(IOrderedQueryable<T> query);
}