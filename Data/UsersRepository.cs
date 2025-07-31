using Microsoft.EntityFrameworkCore;
using UserService.Dtos;
using UserService.Extentions;
using UserService.Models;

namespace UserService.Data
{
    public class UsersRepository : IUserRepository
    {
        private readonly UsersDbContext _dbContext;

        public UsersRepository(UsersDbContext context)
        {
            _dbContext = context;
        }

        public async Task AddUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _dbContext.Users.AddAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            _dbContext.Users.Remove(user);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbContext.Users.AnyAsync(user => user.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(UserSearchDto query)
        {
            var usersQuery = _dbContext.Users
                .AsQueryable()
                .ApplySearchFilters(query);

            return await usersQuery
                .OrderBy(u => u.Id) 
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();
        }
        
        public async Task<bool> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _dbContext.Users.AnyAsync(user => user.Id == id);
        }

        public async Task<int> CountUsersAsync(UserSearchDto query)
        {
            var usersQuery = _dbContext.Users
                .AsQueryable()
                .ApplySearchFilters(query);
            
            return await usersQuery.CountAsync();
        }
    }
}