<<<<<<<< HEAD:src/CourseService/Application/Services/LinkProvider/Abstractions/LinkDto.cs
namespace Courses.Application.Services.LinkProvider.Abstractions;
========
namespace Courses.Application.Abstractions.Hateoas;
>>>>>>>> origin/main:src/CourseService/Application/Abstractions/Hateoas/LinkDto.cs

public sealed record LinkDto
{
    public required string Href { get; init; }
    public required string Rel { get; init; }
    public required string Method { get; set; }
}
