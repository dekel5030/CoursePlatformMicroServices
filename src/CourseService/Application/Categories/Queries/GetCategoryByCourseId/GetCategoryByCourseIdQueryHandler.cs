using Courses.Application.Categories.Dtos;
using Courses.Application.Categories.Queries.GetByIds;
using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetById;
using Courses.Domain.Categories.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Categories.Queries.GetCategoryByCourseId;

public sealed class GetCategoryByCourseIdQueryHandler
    : IQueryHandler<GetCategoryByCourseIdQuery, CategoryDto>
{
    private readonly IMediator _mediator;

    public GetCategoryByCourseIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Result<CategoryDto>> Handle(
        GetCategoryByCourseIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Result<CourseDto> courseResult = await _mediator.Send(
            new GetCourseByIdQuery(request.CourseId),
            cancellationToken);

        if (courseResult.IsFailure)
        {
            return Result<CategoryDto>.Failure(courseResult.Error);
        }

        CourseDto course = courseResult.Value;

        Result<IReadOnlyList<CategoryDto>> categoriesResult = await _mediator.Send(
            new GetCategoriesByIdsQuery([course.CategoryId]),
            cancellationToken);

        if (categoriesResult.IsFailure)
        {
            return Result<CategoryDto>.Failure(categoriesResult.Error);
        }

        IReadOnlyList<CategoryDto> categories = categoriesResult.Value;

        CategoryDto? category = categories.Count > 0 ? categories[0] : null;
        
        if (category is null)
        {
            return Result<CategoryDto>.Failure(CategoryErrors.NotFound);
        }

        return Result<CategoryDto>.Success(category);
    }
}
