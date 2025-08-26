using AutoMapper;
using Common;
using Common.Errors;
using Common.Messaging.EventEnvelope;
using Users.Contracts.Events;
using UserService.Data;
using UserService.Dtos;
using UserService.Messaging.Publishers;
using UserService.Models;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly IUserEventPublisher _eventPublisher;
    private readonly string _producer = "UserService";

    public UserService(IUserRepository repository, IMapper mapper, IUserEventPublisher eventPublisher)
    {
        _repository = repository;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
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
        await PublishUserUpserted(user, isActive: true);

        var userReadDto = _mapper.Map<UserReadDto>(user);
        return Result<UserReadDto>.Success(userReadDto);
    }

    // === GET ===

    public async Task<UserDetailsDto?> GetUserByIdAsync(int id)
    {
        var user = await _repository.GetUserByIdAsync(id);

        return _mapper.Map<UserDetailsDto>(user);
    }

    public async Task<PagedResponseDto<UserReadDto>> GetUsersByQueryAsync(UserSearchDto query)
    {
        var users = await _repository.SearchUsersAsync(query);
        var totalCount = await _repository.CountUsersAsync(query);

        return new PagedResponseDto<UserReadDto>
        {
            Items = _mapper.Map<IEnumerable<UserReadDto>>(users),
            TotalCount = totalCount,
            PageSize = query.PageSize,
            PageNumber = query.PageNumber
        };
    }

    // === Update ===
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
        await PublishUserUpserted(user, isActive: false);

        return Result<UserReadDto>.Success(_mapper.Map<UserReadDto>(user));
    }

    private Task PublishUserUpserted(
        User user, bool isActive, string? correlationId = null, string? eventId = null, CancellationToken ct = default)
    {
        var payload = new UserUpsertedV1(UserId: user.Id, IsActive: isActive);
        var envelope = EventEnvelope<UserUpsertedV1>
                        .Create(
                            producer: _producer,
                            aggregateId: user.Id.ToString(),
                            aggregateVersion: user.AggregateVersion,
                            payload: payload,
                            contractVersion: UserUpsertedV1.Version.ToString(),
                            correlationId: correlationId,
                            eventId: eventId
                        );
        
        return _eventPublisher.PublishUserUpsertedAsync(envelope, ct);
    }
}
