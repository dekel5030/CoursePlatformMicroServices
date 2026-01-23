using Courses.Application.Actions.Abstractions;
using Kernel.Auth.Abstractions;

namespace Courses.Application.Actions.CourseCollection.Policies;

public interface ICourseCollectionActionRule : IActionRule<CourseCollectionAction, Empty>;
