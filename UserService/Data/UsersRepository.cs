using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data
{
    public class UsersRepository : IUserRepository
    {
        private readonly UsersDbContext _context;

        public UsersRepository(UsersDbContext context)
        {
            _context = context;
        }
        public async Task ActivateUserAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);

            if (user != null)
            {
                user.IsActive = true;
            }
        }
        
        public async Task DeactivateUserAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);

            if (user != null)
            {
                user.IsActive = false;
            }
        }

        public async Task AddUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _context.Users.AddAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            _context.Users.Remove(user);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(user => user.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> IsEmailConfirmedAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            
            return user.EmailConfirmed;
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return Enumerable.Empty<User>();
            }

            return await _context.Users
                .Where(user => user.FullName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                               user.Email.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }
        
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(user => user.Id == id);
        }
    }
}