using Courses.Application.Shared.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Features.CourseCatalog;

public sealed record CourseCatalogQuery(PagedQueryDto PagedQuery) : IQuery<CourseCollectionDto>;
