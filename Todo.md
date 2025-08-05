### ✅ Cleanup & Maintainability

- Create a generic extension method to configure `CreatedAt` and `UpdatedAt` properties
  for all entities inheriting from `BaseEntity`.
- Replace manual configuration in `CourseDbContext` with a call to `ConfigureBaseEntity()`.
- Register CourseDbContext to DI
- Register repos DI
