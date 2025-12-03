using Domain.Courses;
using Domain.Enrollments;
using Domain.Enrollments.Events;
using Domain.Enrollments.Primitives;
using FluentAssertions;
using Kernel;
using Xunit;

namespace Domain.UnitTests;

public class CourseEnrollmentTests
{
    private readonly Course _testCourse;

    public CourseEnrollmentTests()
    {
        _testCourse = Course.CreateCourse(
            "Test Course",
            "Test Description",
            "https://test.com/image.jpg",
            "instructor-123",
            new Money(100, "ILS")
        );
    }

    [Fact]
    public void EnrollStudent_ShouldCreateEnrollmentAndAddToCollection()
    {
        // Arrange
        var studentId = Guid.NewGuid();

        // Act
        var enrollment = _testCourse.EnrollStudent(studentId);

        // Assert
        enrollment.Should().NotBeNull();
        enrollment.StudentId.Should().Be(studentId);
        enrollment.CourseId.Should().Be(_testCourse.Id);
        enrollment.CompletionStatus.Should().Be(CompletionStatus.NotStarted);

        _testCourse.Enrollments.Should().HaveCount(1);
        _testCourse.Enrollments.Should().Contain(enrollment);
    }

    [Fact]
    public void EnrollStudent_ShouldRaiseStudentEnrolledDomainEvent()
    {
        // Arrange
        var studentId = Guid.NewGuid();

        // Act
        var enrollment = _testCourse.EnrollStudent(studentId);

        // Assert
        var domainEvents = enrollment.DomainEvents;
        domainEvents.Should().HaveCount(1);
        domainEvents.First().Should().BeOfType<StudentEnrolledDomainEvent>();
    }

    [Fact]
    public void EnrollStudent_WhenStudentAlreadyEnrolled_ShouldReturnExistingEnrollment()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var firstEnrollment = _testCourse.EnrollStudent(studentId);

        // Act
        var secondEnrollment = _testCourse.EnrollStudent(studentId);

        // Assert
        secondEnrollment.Should().Be(firstEnrollment);
        _testCourse.Enrollments.Should().HaveCount(1);
    }

    [Fact]
    public void EnrollStudent_MultipleDifferentStudents_ShouldAddEachToCollection()
    {
        // Arrange
        var studentId1 = Guid.NewGuid();
        var studentId2 = Guid.NewGuid();
        var studentId3 = Guid.NewGuid();

        // Act
        var enrollment1 = _testCourse.EnrollStudent(studentId1);
        var enrollment2 = _testCourse.EnrollStudent(studentId2);
        var enrollment3 = _testCourse.EnrollStudent(studentId3);

        // Assert
        _testCourse.Enrollments.Should().HaveCount(3);
        _testCourse.Enrollments.Should().Contain(enrollment1);
        _testCourse.Enrollments.Should().Contain(enrollment2);
        _testCourse.Enrollments.Should().Contain(enrollment3);
    }

    [Fact]
    public void Enrollments_ShouldBeReadOnlyCollection()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        _testCourse.EnrollStudent(studentId);

        // Assert - Verify that Enrollments is IReadOnlyCollection
        _testCourse.Enrollments.Should().BeAssignableTo<IReadOnlyCollection<Enrollment>>();
    }
}
