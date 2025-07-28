using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class User
    {
        [Key]
        public int Id { get; private set; }

        public required string Email { get; set; }
        public string PasswordHash { get; private set; }
        public bool EmailConfirmed { get; internal set; }

        public required string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Bio { get; set; }

        public string? ProfileImageSmallUrl { get; set; }
        public string? ProfileImageMediumUrl { get; set; }
        public string? ProfileImageLargeUrl { get; set; }

        public bool IsActive { get; internal set; }
        public string? Role { get; internal set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private User()
        {
            PasswordHash = string.Empty;
        }

        internal void SetPasswordHash(string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                throw new ArgumentException("Password hash cannot be null or empty.", nameof(hash));
            }

            PasswordHash = hash;
        }
    }
}
