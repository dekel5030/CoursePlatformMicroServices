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

        public async Task<Result<UserReadDto>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _repository.GetUserByIdAsync(id);
                
                if (user == null)
                {
                    return Result<UserReadDto>.Failure(ErrorCode.UserNotFound);
                }

                return Result<UserReadDto>.Success(_mapper.Map<UserReadDto>(user));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Getting user by ID failed: {ex.Message}");
                return Result<UserReadDto>.Failure(ErrorCode.DatabaseError);
            }
        }

        public async Task<Result<UserReadDto>> CreateUserAsync(UserCreateDto userCreateDto)
        {
            var user = _mapper.Map<User>(userCreateDto);

            try
            {
                if (await _repository.EmailExistsAsync(user.Email))
                {
                    return Result<UserReadDto>.Failure(ErrorCode.DuplicateEmail);
                }

                await _repository.AddUserAsync(user);
                await _repository.SaveChangesAsync();
                var userReadDto = _mapper.Map<UserReadDto>(user);

                return Result<UserReadDto>.Success(userReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Creating user failed: {ex.Message}");
                return Result<UserReadDto>.Failure(ErrorCode.DatabaseError);
            }
        }

        public Task<Result<UserReadDto>> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = _repository.GetUserByEmailAsync(email);
                
                if (user == null)
                {
                    return Task.FromResult(Result<UserReadDto>.Failure(ErrorCode.UserNotFound));
                }

                return Task.FromResult(Result<UserReadDto>.Success(_mapper.Map<UserReadDto>(user)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Getting user by email failed: {ex.Message}");
                return Task.FromResult(Result<UserReadDto>.Failure(ErrorCode.DatabaseError));
            }
        }

        public Task<Result<IEnumerable<UserReadDto>>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<UserReadDto>>> SearchUsersAsync(string query)
        {
            try
            {
                var users = _repository.SearchUsersAsync(query);
                var userDtos = _mapper.Map<IEnumerable<UserReadDto>>(users);
                return Task.FromResult(Result<IEnumerable<UserReadDto>>.Success(userDtos));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Searching users failed: {ex.Message}");
                return Task.FromResult(Result<IEnumerable<UserReadDto>>.Failure(ErrorCode.DatabaseError));
            }
        }

        public Task<Result<UserReadDto>> ActivateUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<UserReadDto>> DeactivateUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<UserReadDto>> DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<UserReadDto>> EmailExistsAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Result<UserReadDto>> UserExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<UserReadDto>> IsEmailConfirmedAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}