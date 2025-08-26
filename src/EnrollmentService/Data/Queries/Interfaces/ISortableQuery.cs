namespace EnrollmentService.Data.Queries.Interfaces;

public interface ISortableQuery<T>
{
    IOrderedQueryable<T> ApplySorting(IQueryable<T> query);
}