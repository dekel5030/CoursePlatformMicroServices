using Courses.Application.Tasks.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Tasks.Queries.GetAllTasks;

public sealed record GetAllTasksQuery() : IQuery<IReadOnlyList<MediaProcessingTaskDto>>;
