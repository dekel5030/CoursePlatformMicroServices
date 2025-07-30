namespace UserService.Dtos
{
    public class UserReadDto
    {
        public required int Id { get; set; }
        public required string Email { get; set; } 
        public required string FullName { get; set; } 
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Bio { get; set; }
        public string? ProfileImageSmallUrl { get; set; }
        public string? ProfileImageMediumUrl { get; set; }
        public string? ProfileImageLargeUrl { get; set; }
        public string? Role { get; set; }
    }
}