using Courses.Domain.Categories.Primitives;
using Courses.Domain.Shared;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Domain.Categories;

public class Category : Entity<CategoryId>
{
    public override CategoryId Id { get; protected set; }
    public string Name { get; private set; }
    public Slug Slug { get; private set; }

#pragma warning disable S1133
#pragma warning disable CS8618
    [Obsolete("This constructor is for EF Core only.", error: true)]
    private Category() { }
#pragma warning restore CS8618
#pragma warning restore S1133

    private Category(CategoryId id, string name, Slug slug)
    {
        Id = id;
        Name = name;
        Slug = slug;
    }

    public static Result<Category> Create(string name)
    {
        var slug = new Slug(name);

        var category = new Category(CategoryId.CreateNew(), name, slug);

        category.Raise(new CategoryCreatedDomainEvent(category.Id, category.Name, category.Slug));

        return Result.Success(category);
    }

    public Result Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            return Result.Failure(Error.Validation("Category.NameEmpty", "Name cannot be empty"));
        }

        if (Name == newName)
        {
            return Result.Success();
        }

        Name = newName;
        Slug = new Slug(newName);

        Raise(new CategoryRenamedDomainEvent(Id, Name, Slug));
        return Result.Success();
    }
}
