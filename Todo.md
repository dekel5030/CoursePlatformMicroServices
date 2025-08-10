### AuthService

- CancellationToken
- Check if db and repo layers handling lower casing as supposed
- Introuduce cacheing in order to reduce db request for roles and permisisons
- Refactor GetCurrentUser in AuthService.cs
- Refactor the using of rollback to handle dadless son
- Refactor services DI Validation settings
- Implement Adding of Permissions, RolePermissions, Roles, and UserPermissions from admin panel
- Maybe save permission claims in int format for efficency role id instead of role name
- validators for admin dtos
- Verify if cascade on deleting a Permission
- Consider switching from per-permission claim model to role-based model
- Refactor AuthService
- Examin if on assign roles and permission should return ok or created at
- Register DI IAdminUserService
- Add mapping profiles for IAadminUserService methods
- Query Object Pattern
- update tokenservice
- Add swagger
