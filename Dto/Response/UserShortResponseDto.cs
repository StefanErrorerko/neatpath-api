using NeatPath.Models.Enums;
using NeatPath.Models;

namespace NeatPath.Dto.Response
{
    public class UserShortResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
