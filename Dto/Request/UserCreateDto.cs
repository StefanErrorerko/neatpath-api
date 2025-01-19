using NeatPath.Models.Enums;
using NeatPath.Models;
using System.Security;
using System.ComponentModel.DataAnnotations;

namespace NeatPath.Dto.Request
{
    public class UserCreateDto
    {
        [StringLength(20, MinimumLength = 3)]
        public string? Username { get; set; }

        [StringLength(50, MinimumLength = 6)]
        public string? Password { get; set; }

        public UserRole? Role { get; set; }
    }
}
