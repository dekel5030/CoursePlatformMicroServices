using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Features.CourseCatalog;

internal sealed record CourseCatalogQuery(PagedQueryDto PagedQuery) : IQuery<CourseCollectionDto>;
