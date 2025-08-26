namespace AuthService.Dtos.Roles
{
    public class RoleSearchDto
    {
        public string? Name { get; set; }
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
}