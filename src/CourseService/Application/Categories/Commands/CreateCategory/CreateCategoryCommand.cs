using Kernel.Messaging.Abstractions;

namespace Courses.Application.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(string Name) : ICommand<CreateCategoryResponse>;
