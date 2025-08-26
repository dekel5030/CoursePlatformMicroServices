### AuthService

- CancellationToken
- Refactor the using of rollback to handle dadless son
- Refactor services DI Validation settings
- Query Object Pattern

### Performance

- Introuduce cacheing in order to reduce db request for roles and permisisons
- Maybe save permission claims in int format for efficency role id instead of role name
- Consider switching from per-permission claim model to role-based model
