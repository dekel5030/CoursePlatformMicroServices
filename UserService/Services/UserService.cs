using AutoMapper;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserReadDto?> GetUserByIdAsync(int id)
        {
            var user = await _repository.GetUserByIdAsync(id);

            return _mapper.Map<UserReadDto>(user);
        }

        public Task ActivateUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<UserReadDto?> CreateUserAsync(UserCreateDto userCreateDto)
        {
            if (userCreateDto == null)
            {
                throw new ArgumentNullException(nameof(userCreateDto));
            }
            var user = _mapper.Map<User>(userCreateDto);
            try
            {
                await _repository.AddUserAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking email existence: {ex.Message}");
                return null;
            }
            
            var userReadDto = _mapper.Map<UserReadDto>(user);

            await _repository.SaveChangesAsync();

            return userReadDto;
        }

        public Task DeactivateUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EmailExistsAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserReadDto>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserReadDto?> GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsEmailConfirmedAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserReadDto>> SearchUsersAsync(string query)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExistsAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}