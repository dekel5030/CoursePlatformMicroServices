namespace UserService.Dtos
{
    public class UserDetailsDto
    {
        public required int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}