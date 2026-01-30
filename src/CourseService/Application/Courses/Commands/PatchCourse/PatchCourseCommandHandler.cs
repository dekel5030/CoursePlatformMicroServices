using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Categories;
using Courses.Domain.Categories.Errors;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.PatchCourse;

internal sealed class PatchCourseCommandHandler : ICommandHandler<PatchCourseCommand>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PatchCourseCommandHandler(
        ICourseRepository courseRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        PatchCourseCommand request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

        if (course is null)
        {
            return Result.Failure(CourseErrors.NotFound);
        }

        course.UpdateDetails(request.Title, request.Description, request.Price);

        if (request.CategoryId != null)
        {
            Category? category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

            if (category is null)
            {
                return Result.Failure(CategoryErrors.NotFound);
            }
        }

        var tags = request.Tags?.Select(Tag.Create).ToList();

        Slug? slug = request.Slug == null ? null : new Slug(request.Slug);

        course.UpdateMetadata(request.Difficulty, request.CategoryId, request.Language, tags, slug);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
