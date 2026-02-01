using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Categories;
using Courses.Domain.Categories.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Categories.Commands.UpdateCategory;

internal sealed class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken = default)
    {
        Category? category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound);
        }

        Result renameResult = category.Rename(request.Name);

        if (renameResult.IsFailure)
        {
            return renameResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
