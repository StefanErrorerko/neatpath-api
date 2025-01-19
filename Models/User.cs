using System.ComponentModel.DataAnnotations;
using System.Security;
using NeatPath.Models.Enums;
using NeatPath.Models.Interfaces;

namespace NeatPath.Models
{
    public class User : ITrackable
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public UserRole Role { get; set; } = UserRole.Anonymous;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Session> Sessions { get; set; }
        public ICollection<Url> Urls { get; set; }
    }
}
