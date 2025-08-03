namespace UserService.Dtos
{
    public class UserReadDto
    {
        public required int Id { get; set; }
        public required string Email { get; set; } 
        public required string FullName { get; set; } 
    }
}