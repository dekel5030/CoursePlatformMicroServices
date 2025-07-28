using AutoMapper;
using UserService.Common.Errors;
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
        
        // === Create ===

        public async Task<Result<UserReadDto>> CreateUserAsync(UserCreateDto userCreateDto)
        {
            var user = _mapper.Map<User>(userCreateDto);

            if (await _repository.EmailExistsAsync(user.Email))
            {
                return Result<UserReadDto>.Failure(ErrorCode.DuplicateEmail);
            }

            await _repository.AddUserAsync(user);
            await _repository.SaveChangesAsync();
            var userReadDto = _mapper.Map<UserReadDto>(user);

            return Result<UserReadDto>.Success(userReadDto);
        }

        // === GET ===

        public async Task<UserReadDto?> GetUserByIdAsync(int id)
        {
            var user = await _repository.GetUserByIdAsync(id);

            return _mapper.Map<UserReadDto>(user);
        }

        public async Task<UserReadDto?> GetUserByEmailAsync(string email)
        {
            var user = await _repository.GetUserByEmailAsync(email);

            return _mapper.Map<UserReadDto>(user);
        }

        public Task<IEnumerable<UserReadDto>> GetPagedUsersAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserReadDto>> SearchUsersAsync(string query)
        {
            var users = await _repository.SearchUsersAsync(query);
            var userDtos = _mapper.Map<IEnumerable<UserReadDto>>(users);

            return userDtos;
        }

        // === Update ===

        public async Task<Result<UserReadDto>> SetUserActivationAsync(int userId, bool isActive)
        {
            var user = await _repository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return Result<UserReadDto>.Failure(ErrorCode.UserNotFound);
            }

            user.IsActive = isActive;
            await _repository.SaveChangesAsync();

            return Result<UserReadDto>.Success(_mapper.Map<UserReadDto>(user));
        }

        // === Delete ===

        public async Task<Result<UserReadDto>> DeleteUserAsync(int userId)
        {
            var user = await _repository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return Result<UserReadDto>.Failure(ErrorCode.UserNotFound);
            }

            await _repository.DeleteUserAsync(userId);
            await _repository.SaveChangesAsync();

            return Result<UserReadDto>.Success(_mapper.Map<UserReadDto>(user));
        }

        // === Validation ===

        public async Task<bool> EmailExistsAsync(string email)
        {
            var exists = await _repository.EmailExistsAsync(email);

            return exists;
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            var user = await _repository.GetUserByIdAsync(id);

            return user != null;
        }

        public async Task<bool> IsEmailConfirmedAsync(int userId)
        {
            var user = await _repository.GetUserByIdAsync(userId);

            return user != null && user.EmailConfirmed;
        }
    }
}