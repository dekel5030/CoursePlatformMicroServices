namespace EnrollmentService.Data.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> UserExistsAsync(int userId, CancellationToken ct = default);
}