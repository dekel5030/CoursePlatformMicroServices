using System.ComponentModel.DataAnnotations;
using Common.Messaging.EventEnvelope;

namespace UserService.Models
{
    public class User : IVersionedEntity
    {
        [Key]
        public int Id { get; private set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Bio { get; set; }

        public string? ProfileImageSmallUrl { get; set; }
        public string? ProfileImageMediumUrl { get; set; }
        public string? ProfileImageLargeUrl { get; set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public long AggregateVersion { get; set; }
        public DateTimeOffset UpdatedAtUtc { get; set; }
    }
}
