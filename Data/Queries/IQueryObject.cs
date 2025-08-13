namespace EnrollmentService.Data.Queries;

public interface IQueryObject<T>
{
    IQueryable<T> Apply(IQueryable<T> query);
}