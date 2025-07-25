using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public bool EmailConfirmed { get; set; } = false;

        public required string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Bio { get; set; }

        public string? ProfileImageSmallUrl { get; set; }
        public string? ProfileImageMediumUrl { get; set; }
        public string? ProfileImageLargeUrl { get; set; }

        public bool IsActive { get; set; } = true;
        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
