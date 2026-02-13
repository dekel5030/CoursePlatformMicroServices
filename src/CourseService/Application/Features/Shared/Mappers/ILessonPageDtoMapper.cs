using Courses.Application.Features.LessonPage;
using Courses.Application.Services.LinkProvider;
using Courses.Domain.Lessons;

namespace Courses.Application.Features.Shared.Mappers;

internal interface ILessonPageDtoMapper
{
    LessonPageDto Map(Lesson lesson, string courseName, LessonContext lessonContext);
}
