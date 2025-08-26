namespace EnrollmentService.Data.Queries.Interfaces;

public interface IQueryObject<T>
{
    IQueryable<T> Apply(IQueryable<T> query);
}