using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace Architecture.Tests;

public class ArchitectureTests
{
    private const string _domainNamespace = "Domain";
    private const string _applicationNamespace = "Application";
    private const string _infrastructureNamespace = "Infrastructure";
    private const string _webApiNamespace = "Web.Api";

    [Fact]
    public void Domain_Should_Not_HaveDependenciesOnOtherProject()
    {
        // Arrange
        var assembly = typeof(Domain.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            _applicationNamespace,
            _infrastructureNamespace,
            _webApiNamespace
        };

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot().HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_HaveDependenciesOnOtherProject()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        var otherProjects = new[]
        {
            _infrastructureNamespace,
            _webApiNamespace
        };

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot().HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Should_Not_HaveDependenciesOnOtherProject()
    {
        // Arrange
        var assembly = typeof(Infrastructure.DependencyInjection).Assembly;

        var otherProjects = new[]
        {
            _webApiNamespace
        };

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot().HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainEventHandler_Should_Not_HaveDependecyOnDbContext()
    {
        // Arrange
        var assembly = typeof(Application.DependencyInjection).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That().ImplementInterface(typeof(IDomainEventHandler<>))
            .ShouldNot().HaveDependencyOn(typeof(IApplicationDbContext).Namespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}
