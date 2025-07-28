using AutoMapper;
using UserService.Common;
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

        public async Task<Result<UserReadDto>> CreateUserAsync(UserCreateDto userCreateDto)
        {
            var user = _mapper.Map<User>(userCreateDto);
            user.SetPasswordHash(userCreateDto.Password);

            try
            {
                if (await _repository.EmailExistsAsync(user.Email))
                {
                    return Result<UserReadDto>.Failure(ErrorCode.DuplicateEmail);
                }

                await _repository.AddUserAsync(user);
                await _repository.SaveChangesAsync();
                return Result<UserReadDto>.Success(_mapper.Map<UserReadDto>(user));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Creating user failed: {ex.Message}");
                return Result<UserReadDto>.Failure(ErrorCode.DatabaseError);
            }
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