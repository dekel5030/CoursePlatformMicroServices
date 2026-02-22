using Courses.Application.Abstractions.Data;
using Courses.Application.Tasks.Dtos;
using Courses.Domain.MediaProcessingTask;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Tasks.Queries.GetAllTasks;

internal sealed class GetAllTasksQueryHandler
    : IQueryHandler<GetAllTasksQuery, IReadOnlyList<MediaProcessingTaskDto>>
{
    private readonly IWriteDbContext _writeDbContext;

    public GetAllTasksQueryHandler(IWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    public async Task<Result<IReadOnlyList<MediaProcessingTaskDto>>> Handle(
        GetAllTasksQuery request,
        CancellationToken cancellationToken = default)
    {
        List<MediaProcessingTaskDto> tasks = await _writeDbContext.MediaProcessingTasks
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAtUtc)
            .Select(t => new MediaProcessingTaskDto(
                t.Id.Value,
                t.OriginalLessonId.Value,
                t.AssignedTo != null ? t.AssignedTo.Value : null,
                t.Status,
                t.Message,
                t.InputRawResources.Select(r => r.Path).ToList(),
                t.OutputResources.Select(r => r.Path).ToList(),
                t.CreatedAtUtc,
                t.AssignedAtUtc,
                t.CompletedAtUtc))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<MediaProcessingTaskDto>>(tasks);
    }
}
