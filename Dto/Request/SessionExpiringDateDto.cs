using NeatPath.Attributes;
using System.ComponentModel.DataAnnotations;

namespace NeatPath.Dto.Request
{
    public class SessionExpiringDateDto
    {
        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}
