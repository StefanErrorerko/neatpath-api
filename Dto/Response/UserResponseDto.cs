using NeatPath.Models.Enums;
using NeatPath.Models;
using System.Security;

namespace NeatPath.Dto.Response
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Session> Sessions { get; set; }
        public ICollection<Url> Urls { get; set; }
    }
}
