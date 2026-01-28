using System;
using System.Collections.Generic;
using System.Text;
using Courses.Application.Courses.Dtos;
using Courses.Application.Services.Actions.States;
using Courses.Application.Services.LinkProvider.Abstractions.Factories;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetCoursePage;

internal record GetCoursePageQuery(Guid Id) : IQuery<CoursePageDto>;
