using Courses.Domain.Categories.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(CategoryId Id, string Name) : ICommand;
