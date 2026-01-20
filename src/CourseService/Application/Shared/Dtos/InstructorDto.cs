using System;
using System.Collections.Generic;
using System.Text;
using Courses.Domain.Courses.Primitives;

namespace Courses.Application.Shared.Dtos;

public record InstructorDto(
    UserId Id,
    string FullName,
    string? AvatarUrl
);
