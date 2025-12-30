namespace Courses.Application.Abstractions;

public interface IUrlResolver
{
    string Resolve(string relativePath);
}
