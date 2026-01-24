using Courses.Application.Services.Actions.States;

namespace Courses.Application.Services.LinkProvider.Abstractions.Factories;

internal interface ILessonLinkFactory
{
    IReadOnlyList<LinkDto> CreateLinks(
        CourseState courseState,
        ModuleState moduleState,
        LessonState lessonState,
        EnrollmentState? enrollmentState = null);
}
