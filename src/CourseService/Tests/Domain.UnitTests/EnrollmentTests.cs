using Domain.Enrollments;
using Domain.Enrollments.Events;
using Domain.Enrollments.Primitives;
using Domain.Courses;
using Domain.Courses.Primitives;
using FluentAssertions;
using Kernel;
using Xunit;

namespace Domain.UnitTests;

public class EnrollmentTests
{
    [Fact]
    public void Create_ShouldCreateEnrollmentWithCorrectProperties()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = new CourseId(Guid.NewGuid());

        // Act
        var enrollment = Enrollment.Create(studentId, courseId);

        // Assert
        enrollment.Id.Value.Should().NotBeEmpty();
        enrollment.StudentId.Should().Be(studentId);
        enrollment.CourseId.Should().Be(courseId);
        enrollment.CompletionStatus.Should().Be(CompletionStatus.NotStarted);
        enrollment.EnrollmentDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldRaiseStudentEnrolledDomainEvent()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var courseId = new CourseId(Guid.NewGuid());

        // Act
        var enrollment = Enrollment.Create(studentId, courseId);

        // Assert
        var domainEvents = enrollment.DomainEvents;
        domainEvents.Should().HaveCount(1);
        domainEvents.First().Should().BeOfType<StudentEnrolledDomainEvent>();

        var domainEvent = (StudentEnrolledDomainEvent)domainEvents.First();
        domainEvent.EnrollmentId.Should().Be(enrollment.Id);
        domainEvent.StudentId.Should().Be(studentId);
        domainEvent.CourseId.Should().Be(courseId);
        domainEvent.EnrollmentDate.Should().Be(enrollment.EnrollmentDate);
    }
}
