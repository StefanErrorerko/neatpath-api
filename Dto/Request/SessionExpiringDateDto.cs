using NeatPath.Attributes;
using System.ComponentModel.DataAnnotations;

namespace NeatPath.Dto.Request
{
    public class SessionExpiringDateDto
    {
        [Required]
        [FutureDate]
        public DateTime ExpiresAt { get; set; }
    }
}
