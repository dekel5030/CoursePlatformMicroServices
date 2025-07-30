using AutoMapper;
using Common;
using Common.Errors;
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
            if (await _repository.EmailExistsAsync(userCreateDto.Email))
            {
                return Result<UserReadDto>.Failure(Error.DuplicateEmail);
            }

            var user = _mapper.Map<User>(userCreateDto);
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

        public async Task<IEnumerable<UserDetailsDto>> GetUsersByQueryAsync(UserSearchDto query)
        {
            var users = await _repository.SearchUsersAsync(query);
            var userDetailsDtos = _mapper.Map<IEnumerable<UserDetailsDto>>(users);

            return userDetailsDtos;
        }

        // === Update ===

        public async Task<Result<UserReadDto>> SetUserActivationAsync(int userId, bool isActive)
        {
            var user = await _repository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return Result<UserReadDto>.Failure(Error.UserNotFound);
            }

            user.IsActive = isActive;
            await _repository.SaveChangesAsync();

            return Result<UserReadDto>.Success(_mapper.Map<UserReadDto>(user));
        }

        public Task<Result<UserReadDto>> PatchUser(UserPatchDto userPatchDto) 
        {
            throw new NotImplementedException();
        }

        // === Delete ===

        public async Task<Result<UserReadDto>> DeleteUserAsync(int userId)
        {
            var user = await _repository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return Result<UserReadDto>.Failure(Error.UserNotFound);
            }

            await _repository.DeleteUserAsync(userId);
            await _repository.SaveChangesAsync();

            return Result<UserReadDto>.Success(_mapper.Map<UserReadDto>(user));
        }
    }
}