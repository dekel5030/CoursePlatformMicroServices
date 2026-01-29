using System;
using System.Collections.Generic;
using System.Text;

namespace CoursePlatform.Contracts.CourseService;

public record ModuleCreatedIntegrationEvent(
    Guid ModuleId,
    Guid CourseId,
    string Title,
    int Index);

public record ModuleTitleChangedIntegrationEvent(
    Guid ModuleId,
    Guid CourseId,
    string NewTitle);

public record ModuleIndexUpdatedIntegrationEvent(
    Guid ModuleId,
    Guid CourseId,
    int NewIndex);

public record ModuleDeletedIntegrationEvent(
    Guid ModuleId,
    Guid CourseId);