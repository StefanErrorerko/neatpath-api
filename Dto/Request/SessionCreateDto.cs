using NeatPath.Attributes;
using NeatPath.Models;
using System.ComponentModel.DataAnnotations;

namespace NeatPath.Dto.Request
{
    public class SessionCreateDto
    {
        [Required]
        public string? Token { get; set; }

        [Required]
        [FutureDate]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
