using Courses.Application.Services.Actions.Abstractions;

namespace Courses.Application.Services.Actions.CourseCollection.Policies;

public interface ICourseCollectionActionRule : IActionRule<CourseCollectionAction, Empty>;
