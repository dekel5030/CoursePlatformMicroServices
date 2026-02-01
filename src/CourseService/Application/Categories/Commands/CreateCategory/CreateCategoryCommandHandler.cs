using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Categories;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Categories.Commands.CreateCategory;

internal sealed class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, CreateCategoryResponse>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateCategoryResponse>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken = default)
    {
        Result<Category> categoryResult = Category.Create(request.Name);

        if (categoryResult.IsFailure)
        {
            return Result.Failure<CreateCategoryResponse>(categoryResult.Error);
        }

        Category category = categoryResult.Value;

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateCategoryResponse(category.Id.Value, category.Name));
    }
}
