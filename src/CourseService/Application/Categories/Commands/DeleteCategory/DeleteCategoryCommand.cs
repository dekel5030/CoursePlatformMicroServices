using Courses.Domain.Categories.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Categories.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand(CategoryId Id) : ICommand;
