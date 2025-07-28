namespace UserService.Dtos
{
    public class UserCreateDto
    {
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public required string PasswordHash { get; set; }
    }
}