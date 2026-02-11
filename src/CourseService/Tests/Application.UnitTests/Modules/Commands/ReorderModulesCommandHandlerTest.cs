using Courses.Application.Abstractions.Data;
using Courses.Application.Modules.Commands.ReorderModules;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Modules;
using Courses.Domain.Modules.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Moq;
using FluentAssertions;
using Xunit;

namespace Application.UnitTests.Modules.Commands;

public class ReorderModulesCommandHandlerTest
{
    private readonly Mock<IModuleRepository> _moduleRepositoryMock;
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ReorderModulesCommandHandler _handler;

    public ReorderModulesCommandHandlerTest()
    {
        _moduleRepositoryMock = new Mock<IModuleRepository>();
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        var moduleManagement = new ModuleManagementService(
            _moduleRepositoryMock.Object,
            _courseRepositoryMock.Object);
        _handler = new ReorderModulesCommandHandler(moduleManagement, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReorderModules_AndSaveChanges()
    {
        var courseId = new CourseId(Guid.NewGuid());
        var module1Result = Module.Create(courseId, 0, new Courses.Domain.Shared.Primitives.Title("Module 1"));
        var module2Result = Module.Create(courseId, 1, new Courses.Domain.Shared.Primitives.Title("Module 2"));
        module1Result.IsSuccess.Should().BeTrue();
        module2Result.IsSuccess.Should().BeTrue();
        var modules = new List<Module> { module1Result.Value!, module2Result.Value! };
        var moduleIds = new List<ModuleId> { module2Result.Value!.Id, module1Result.Value!.Id };
        var command = new ReorderModulesCommand(courseId, moduleIds);

        _moduleRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Module, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(modules);
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        Result result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenModuleCountMismatch()
    {
        var courseId = new CourseId(Guid.NewGuid());
        var module1Result = Module.Create(courseId, 0, new Courses.Domain.Shared.Primitives.Title("Module 1"));
        module1Result.IsSuccess.Should().BeTrue();
        var modules = new List<Module> { module1Result.Value! };
        var moduleIds = new List<ModuleId> { module1Result.Value!.Id, new ModuleId(Guid.NewGuid()) };
        var command = new ReorderModulesCommand(courseId, moduleIds);

        _moduleRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Module, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(modules);

        Result result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Contain("InvalidOrderCount");
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
