### AuthService

- CancellationToken
- Check if db and repo layers handling lower casing as supposed
- Introuduce cacheing in order to reduce db request for roles and permisisons
- Refactor GetCurrentUser in AuthService.cs
- Refactor the using of rollback to handle dadless son
- Refactor services DI Validation settings
- Implement Adding of Permissions, RolePermissions, Roles, and UserPermissions from admin panel
- validators for admin dtos
- implement search by query in GetPermissionsAsync method in permissionRepo
